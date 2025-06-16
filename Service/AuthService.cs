using System.Security.Cryptography;
using System.Text;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMailService _mailService;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly string _emailSecureCharacters;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IMailService mailService,
        IJwtService jwtService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mailService = mailService;
        _jwtService = jwtService;
        _configuration = configuration;
        _emailSecureCharacters = _configuration["MailSettings:SecureCharacters"] ?? "";
    }

    public async Task<string> SeedRolesAsync()
    {
        var existingRoles = await _roleRepository.GetAllAsync();

        if (existingRoles.Any())
        {
            return "Roles are already initialized.";
        }

        var roles = new List<Role>
        {
            new Role { Name = "Admin", Description = "Quản trị hệ thống, quản lý người dùng, nội dung và thống kê toàn hệ thống."},
            new Role { Name = "Officer", Description = "Cán bộ địa phương, xác minh báo cáo, xử lý sự cố và theo dõi khu vực." },
            new Role { Name = "Citizen", Description = "Người dùng thông thường, gửi báo cáo, nhận cảnh báo và tham gia cộng đồng.\r\n" }
        };

        foreach (var role in roles)
        {
            await _roleRepository.CreateAsync(role);
        }

        return "Roles initialized successfully.";
    }

    public async Task Register(UserRegistrationRequestModel userDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUser != null)
        {
            throw new Exception("Account already exists.");
        }

        var customerRole = await _roleRepository.GetByNameAsync("Citizen");

        if (customerRole == null)
        {
            throw new Exception("Citizen role does not exist. Please initialize roles first.");
        }

        string verificationCode = GenerateActivationCode();

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Email = userDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            FullName = userDto.FullName,
            DateOfBirth = userDto.DateOfBirth.ToUniversalTime(),
            Phone = userDto.Phone,
            TotalPoint = 0,
            Gender = userDto.Gender,
            Status = "inactive",
            RoleId = customerRole.Id,
            ImageUrl = "",
            ActivationCode = verificationCode,
            RefreshTokenExpiry = null,
            RefreshToken = null,
            IsLoggedIn = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
        };


        await _userRepository.CreateAsync(account);
        string subject = "Your Verification Code";
        string message = $"<h2>Welcome to Our Service</h2><p>Your verification code is: <b>{verificationCode}</b></p>";
        await _mailService.SendEmailVerificationCode(userDto.Email, subject, message);
    }

    private string GenerateActivationCode()
    {
        StringBuilder codeBuilder = new StringBuilder();
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] randomBytes = new byte[1];

            for (int i = 0; i < 6; i++)
            {
                rng.GetBytes(randomBytes);
                int randomIndex = randomBytes[0] % _emailSecureCharacters.Length;
                codeBuilder.Append(_emailSecureCharacters[randomIndex]);
            }
        }

        return codeBuilder.ToString();
    }

    public async Task VerifyOtp(string email, string otp)
    {
        if (string.IsNullOrWhiteSpace(otp))
        {
            throw new Exception("OTP cannot be empty.");
        }
        else if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Email cannot be empty.");
        }

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new KeyNotFoundException("Account not found.");
        }

        if (user.ActivationCode != otp)
        {
            throw new UnauthorizedAccessException("Incorrect OTP. Please try again.");
        }

        user.Status = "active";
        user.ActivationCode = null;
        await _userRepository.UpdateAsync(user);
    }

    public async Task<UserAuthenticationResponse> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new Exception("Email and password cannot be empty.");

        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Incorrect email/password please try again.");
        }
        
        if (user.Status.ToLower() == "inactive" && !string.IsNullOrEmpty(user.ActivationCode))
        {
            throw new UnauthorizedAccessException("Your account is not verified.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = refreshTokenExpiry;
        user.IsLoggedIn = true;
        await _userRepository.UpdateAsync(user);

        return new UserAuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<UserAuthenticationResponse> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Refresh token is required.");

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired, please log in again.");

        var newAccessToken = _jwtService.GenerateAccessToken(user);

        return new UserAuthenticationResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = user.RefreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Refresh token is required.");

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired, please log in again.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.IsLoggedIn = false;

        await _userRepository.UpdateAsync(user);
    }
}