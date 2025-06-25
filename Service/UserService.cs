using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Repository.HandleException;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IFirebaseStorageService _firebaseStorageService;
    private readonly IScanningCardService _scanningCardService;
    private readonly ISubcriptionRepository _subcriptionRepository;

    public UserService(IUserRepository userRepository, IFirebaseStorageService firebaseStorageService,
        IScanningCardService scanningCardService, ISubcriptionRepository subcriptionRepository)
    {
        _userRepository = userRepository;
        _firebaseStorageService = firebaseStorageService;
        _scanningCardService = scanningCardService;
        _subcriptionRepository = subcriptionRepository;
    }

    public async Task<UserProfileResponseModel?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetProfileByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }
        if (user.CitizenIdentityCard == null)
            throw new Exception("User has no identity card record to update.");

        return new UserProfileResponseModel
        {
            FullName = user.FullName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            ImageUrl = user.ImageUrl,
            Gender = user.Gender,
            Phone = user.Phone,
            IsBiometricEnabled = user.IsBiometricEnabled,
            IdNumber = user.CitizenIdentityCard.IdNumber,
            Address = user.CitizenIdentityCard.Address,
            IssueDate = user.CitizenIdentityCard.IssueDate,
            ExpiryDate = user.CitizenIdentityCard.ExpiryDate,
            PlaceOfIssue = user.CitizenIdentityCard.PlaceOfIssue,
            PlaceOfBirth = user.CitizenIdentityCard.PlaceOfBirth,
            TotalPoint = user.TotalPoint,
            CurrentSubscription = await _subcriptionRepository.GetCurrentSubscriptionAsync(user),
            AchievementName = user.Achievement != null ? user.Achievement.Name : "Unrank"
        };
    }

    public async Task<bool> UpdateUserAsync(Guid id, UpdateUserRequestModel userDto, string otp)
    {
        var errors = new Dictionary<string, string>();
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (string.IsNullOrWhiteSpace(user.ActivationCode) || user.ActivationCode != otp)
        {
            errors["otp"] = "Invalid OTP code.";
        }
        else if (!user.CodeExpiry.HasValue || user.CodeExpiry.Value < DateTime.UtcNow)
        {
            errors["otp"] = "OTP code has expired.";
        }

        if (userDto.frontImage != null || userDto.backImage != null)
        {
            ParsedIdCardResult? frontCardInfo = null;
            ParsedIdCardResult? backCardInfo = null;

            if (userDto.frontImage != null)
            {
                frontCardInfo = await _scanningCardService.ParseVietnameseIdCardAsync(userDto.frontImage);
                if (frontCardInfo == null)
                {
                    errors["frontImage"] = "Invalid front image uploaded or not a supported card side.";
                }
                else if (frontCardInfo.CardSideType != "cc_front" && frontCardInfo.CardSideType != "chip_front")
                {
                    errors["frontImage"] = "Front image must be of type 'cc_front' or 'chip_front'.";
                }
            }
            else
            {
                errors["frontImage"] = "Front image is required.";
            }

            if (userDto.backImage != null)
            {
                backCardInfo = await _scanningCardService.ParseVietnameseIdCardAsync(userDto.backImage);
                if (backCardInfo == null)
                {
                    errors["backImage"] = "Invalid back image uploaded or not a supported card side.";
                }
                else if (backCardInfo.CardSideType != "cc_back" && backCardInfo.CardSideType != "chip_back")
                {
                    errors["backImage"] = "Back image must be of type 'cc_back' or 'chip_back'.";
                }
            }
            else
            {
                errors["backImage"] = "Back image is required.";
            }

            if (frontCardInfo != null && backCardInfo != null)
            {
                var frontTypePrefix = frontCardInfo.CardSideType!.Split('_')[0];
                var backTypePrefix = backCardInfo.CardSideType!.Split('_')[0];
                if (frontTypePrefix != backTypePrefix)
                {
                    errors["identityCard"] = "Front and back image types must match: both 'chip' or both 'cc'.";
                }
            }

            if (frontCardInfo != null)
            {
                var duplicateField = await CheckDuplicateFieldsAsync(userDto, id, frontCardInfo.IdNumber);
                if (duplicateField != null)
                {
                    errors["duplicateField"] = duplicateField;
                }
            }

            var identityCard = user.CitizenIdentityCard;
            if (identityCard == null)
            {
                errors["identityCard"] = "User has no identity card record to update.";
            }

            if (errors.Any())
                throw new CustomValidationError(errors);

            string frontImageUrl = await _firebaseStorageService.UploadFileAsync(userDto.frontImage!, "uploads");
            string backImageUrl = await _firebaseStorageService.UploadFileAsync(userDto.backImage!, "uploads");

            user.Email = userDto.email;
            user.Phone = userDto.phone;
            user.FullName = frontCardInfo.FullName;
            user.DateOfBirth = frontCardInfo.DateOfBirth.Value.ToUniversalTime();
            user.Gender = frontCardInfo.Gender ?? user.Gender;

            identityCard.IdNumber = frontCardInfo.IdNumber;
            identityCard.Address = backCardInfo!.Address ?? frontCardInfo.Address;
            identityCard.PlaceOfIssue = backCardInfo.PlaceOfIssue;
            identityCard.PlaceOfBirth = backCardInfo.PlaceOfBirth;
            identityCard.IssueDate = backCardInfo.IssueDate.Value.ToUniversalTime();
            identityCard.ExpiryDate = frontCardInfo.ExpiryDate != null ? frontCardInfo.ExpiryDate.Value.ToUniversalTime()
                : backCardInfo.ExpiryDate.Value.ToUniversalTime();
            identityCard.FrontImageUrl = frontImageUrl;
            identityCard.BackImageUrl = backImageUrl;
            identityCard.UpdatedAt = DateTime.UtcNow;

            user.ActivationCode = null;
            user.CodeExpiry = null;

            await _userRepository.UpdateAsync(user);
            return true;
        }
        else
        {
            var duplicateField = await CheckDuplicateFieldsAsync(userDto, id, "N/A id number");
            if (duplicateField != null)
            {
                errors["duplicateField"] = duplicateField;
            }
            if (errors.Any())
                throw new CustomValidationError(errors);

            user.Email = userDto.email;
            user.Phone = userDto.phone;

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }


    private async Task<string?> CheckDuplicateFieldsAsync(UpdateUserRequestModel userDto, Guid id, string idNumber)
    {
        var user = await _userRepository.GetByEmailAsync(userDto.email);
        if (user != null && !user.Id.Equals(id))
            return "Email already exists.";
        user = await _userRepository.GetByPhoneAsync(userDto.phone);
        if (user != null && !user.Id.Equals(id))
            return "Phone number already exists.";
        user = await _userRepository.GetByIdNumberAsync(idNumber);
        if (user != null && !user.Id.Equals(id))
            return "Cccd number already exists.";
        return null;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
        if (!isOldPasswordValid)
            throw new Exception("Old password is incorrect.");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> UpdateUserImageAsync(Guid id, string imageUrl)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found.");

        user.ImageUrl = imageUrl;
        await _userRepository.UpdateAsync(user);

        return true;
    }
}