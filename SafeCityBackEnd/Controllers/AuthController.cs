using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service;
using SafeCityBackEnd.Helpers;

namespace SafeCityBackEnd.Controllers;

[ApiController]
[Route("api/auth")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;

    private readonly IConfiguration _configuration;
    public AuthController(IAuthService service, IConfiguration configuration, IAccountService accountService)
    {
        _authService = service;
        _configuration = configuration;
        _accountService = accountService;
    }

    [HttpGet("init-roles")]
    public async Task<IActionResult> InitializeRoles()
    {
        var result = await _authService.SeedRolesAsync();
        return Ok(result);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationRequestModel userDto)
    {
        await _authService.Register(userDto);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Accepted, "Successfully Register",
            "Please check your email for account verification.");
    }

    [HttpPost("verify-account")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestModel request)
    {
        try
        {
            await _authService.VerifyOtp(request.Email, request.Otp);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "OTP Verified",
                "Your account is now active.");
        } catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestModel request)
    {
        try
        {
            var authResponse = await _authService.LoginAsync(request.Email, request.Password);

            var responseObject = new
            {
                access_token = authResponse.AccessToken,
                refresh_token = authResponse.RefreshToken
            };

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Login successful", responseObject);
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
    {
        try
        {
            var authResponse = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(new
            {
                access_token = authResponse.AccessToken,
                refresh_token = authResponse.RefreshToken
            });
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        } catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while refreshing the token.", error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestModel request)
    {
        try
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Logout successful",
                "You have been logged out.");
        } catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        } catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while logging out.", error = ex.Message });
        }
    }

    [HttpGet("get-user-info")]
    public async Task<IActionResult> DecodeToken()
    {
        try
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { error = "Authorization header is missing or invalid" });
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var claims = identity.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (email != null && role != null)
                {
                    var accounts = await _accountService.GetAllAsync();
                    var user = accounts.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
                    if (user == null) throw new Exception("Invalid token");
                    return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get user information successful",
                user); ;
                } else
                {
                    return Unauthorized();
                }
            }

            return Unauthorized();
        } catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}