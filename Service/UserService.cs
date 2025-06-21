using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IFirebaseStorageService _firebaseStorageService;
    private readonly IScanningCardService _scanningCardService;

    public UserService(IUserRepository userRepository, IFirebaseStorageService firebaseStorageService, IScanningCardService scanningCardService)
    {
        _userRepository = userRepository;
        _firebaseStorageService = firebaseStorageService;
        _scanningCardService = scanningCardService;
    }

    public async Task<UserProfileResponseModel?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
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
        };
    }

    public async Task<bool> UpdateUserAsync(Guid id, UpdateUserRequestModel userDto, string otp)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found.");

        if (string.IsNullOrWhiteSpace(user.ActivationCode) || user.ActivationCode != otp)
            throw new Exception("Invalid OTP code.");

        if (!user.CodeExpiry.HasValue || user.CodeExpiry.Value < DateTime.UtcNow)
            throw new Exception("OTP code has expired.");

        var duplicateField = await CheckDuplicateFieldsAsync(userDto, id);
        if (duplicateField != null)
            throw new Exception(duplicateField);

        string frontImageUrl = userDto.frontImage != null
            ? await _firebaseStorageService.UploadFileAsync(userDto.frontImage, "uploads")
            : string.Empty;

        string backImageUrl = userDto.backImage != null
            ? await _firebaseStorageService.UploadFileAsync(userDto.backImage, "uploads")
            : string.Empty;
        if (string.IsNullOrWhiteSpace(frontImageUrl) || string.IsNullOrWhiteSpace(backImageUrl))
        {
            user.Phone = userDto.phone;
            user.Email = userDto.email;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        user.FullName = userDto.fullName;
        user.Email = userDto.email;
        user.DateOfBirth = userDto.dateOfBirth.ToUniversalTime();
        user.Gender = userDto.gender;
        user.Phone = userDto.phone;

        var identityCard = user.CitizenIdentityCard;
        if (identityCard == null)
            throw new Exception("User has no identity card record to update.");

        identityCard.IdNumber = userDto.idNumber;
        identityCard.Address = userDto.address;
        identityCard.IssueDate = userDto.issueDate.ToUniversalTime();
        identityCard.ExpiryDate = userDto.expiryDate.ToUniversalTime();
        identityCard.PlaceOfIssue = userDto.placeOfIssue;
        identityCard.PlaceOfBirth = userDto.placeOfBirth;
        identityCard.FrontImageUrl = frontImageUrl;
        identityCard.BackImageUrl = backImageUrl;
        identityCard.UpdatedAt = DateTime.UtcNow;

        user.ActivationCode = null;
        user.CodeExpiry = null;

        await _userRepository.UpdateAsync(user);
        return true;
    }

    private async Task<string?> CheckDuplicateFieldsAsync(UpdateUserRequestModel userDto, Guid id)
    {
        var user = await _userRepository.GetByEmailAsync(userDto.email);
        if (user != null && !user.Id.Equals(id))
            return "Email already exists.";
        user = await _userRepository.GetByPhoneAsync(userDto.phone);
        if (user != null && !user.Id.Equals(id))
            return "Phone number already exists.";
        user = await _userRepository.GetByIdNumberAsync(userDto.idNumber);
        if (user != null && !user.Id.Equals(id))
            return "Cccd number already exists.";
        return null;
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