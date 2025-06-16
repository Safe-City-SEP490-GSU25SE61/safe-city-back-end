using System.Net;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using SafeCityBackEnd.Helpers;

namespace SafeCityBackEnd.Controllers;

[ApiController]
[Route("api/setting")]
[ApiExplorerSettings(GroupName = "Account Settings")]
public class AccountSettingController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AccountSettingController(IConfiguration configuration, IUserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [Authorize]
    [HttpGet("account/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
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
    [HttpPost("account/update/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequestModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            bool updated = await _userService.UpdateUserAsync(id, model);
            if (!updated) return NotFound("User not found.");
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK,
                "Successfully Update Account Information",
                null);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
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