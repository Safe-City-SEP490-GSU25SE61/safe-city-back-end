using BusinessObject.DTOs.Enums;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service;
using Service.Interfaces;
using System.Net;
using System.Security.Claims;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/blogs")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Blog")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly BlogModerationService _blogModerationService;

        public BlogController(IBlogService blogService, BlogModerationService blogModerationService)
        {
            _blogService = blogService;
            _blogModerationService = blogModerationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BlogCreateRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var authorId = Guid.Parse(userIdClaim.Value);
            try
            {
                var result = await _blogService.CreateBlogAsync(request, authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("approve/{id}")]
        public async Task<IActionResult> Approve(int id, bool isApproved, bool isPinned)
        {
            try
            {
                await _blogService.ApproveBlogAsync(id, isApproved, isPinned);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("district/{districtId}")]
        public async Task<IActionResult> GetByDistrict(int districtId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var result = await _blogService.GetBlogsByDistrictAsync(districtId, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blogs by district successfully", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                await _blogService.LikeAsync(userId, postId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("created-blogs")]
        public async Task<IActionResult> GetCreatedBlogsByUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var blogs = await _blogService.GetCreatedBlogsByUserAsync(userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blog history successfully", blogs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogModerationResult>> TestModeration([FromBody]string testContent, BlogType type)
        {
            try
            {
                var result = await _blogModerationService.ModerateBlogAsync(testContent, type);

                return Ok(new
                {
                    testContent,
                    moderationResult = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error occurred during test" });
            }
        }

    }
}
