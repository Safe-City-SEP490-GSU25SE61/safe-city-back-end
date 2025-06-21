using System.Net;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SafeCityBackEnd.Helpers;
using Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SafeCityBackEnd.Controllers;

[ApiController]
[Route("api/settings")]
[ApiExplorerSettings(GroupName = "Account Settings")]
public class AccountSettingController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AccountSettingController(IConfiguration configuration, IUserService userService, IAuthService authService)
    {
        _configuration = configuration;
        _userService = userService;
        _authService = authService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserById()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID claim not found.");

        var userId = Guid.Parse(userIdClaim.Value);
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Accepted,
                "Successfully Retrieve Account Information",
                user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequestModel model, string otp)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("User ID claim not found.");

        var userId = Guid.Parse(userIdClaim.Value);

        try
        {
            bool updated = await _userService.UpdateUserAsync(userId, model, otp);
            if (!updated) return NotFound("User not found.");

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Successfully updated account information.", null);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }


    [Authorize]
    [HttpPost("request-profile-update-otp")]
    public async Task<IActionResult> RequestProfileUpdateOtp(string email)
    {
        await _authService.SendProfileUpdateOtpAsync(email);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Verification code sent to your email.", null);
    }


    //[Authorize]
    //[HttpPost("account/upload-image/{id}")]
    //public async Task<IActionResult> UploadUserImage(int id, IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return BadRequest("No file uploaded.");

    //    try
    //    {
    //        var result = await _uploadImageService.UploadImageAsync(file);
    //        if (result.Error != null)
    //            return BadRequest(new { message = result.Error.Message });

    //        string imageUrl = result.SecureUrl.ToString();
    //        bool updated = await _userService.UpdateUserImageAsync(id, imageUrl);

    //        if (!updated) return NotFound("User not found.");

    //        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
    //            "Successfully uploaded and updated profile image",
    //            new { imageUrl });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, new { message = "An error occurred", error = ex.Message });
    //    }
    //}

}