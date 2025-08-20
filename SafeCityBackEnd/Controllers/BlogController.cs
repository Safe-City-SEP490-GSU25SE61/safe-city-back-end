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

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
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
                var result = await _blogService.CreateBlog(request, authorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("approve/{id}")]
        //[Authorize(Roles = "Officer")]
        public async Task<IActionResult> Approve(int id, bool isApproved, bool isPinned)
        {
            try
            {
                var officerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (officerIdClaim == null)
                    return CustomErrorHandler.SimpleError("User ID claim not found.", 401);
                var officerId = Guid.Parse(officerIdClaim.Value);
                await _blogService.ApproveBlog(id, isApproved, isPinned, officerId);
                return Ok();
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
                await _blogService.Like(userId, postId);
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
                var blogs = await _blogService.GetCreatedBlogsByUser(userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blog history successfully", blogs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("officer")]
        public async Task<IActionResult> GetBlogsByCommune([FromQuery] BlogFilterForOfficerRequest filter)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                var blogs = await _blogService.GetBlogsForOfficer(userId, filter);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blogs for officer successfully", blogs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("officer/{id}")]
        public async Task<IActionResult> GetBlogModerationById(int id)
        {
            try
            {
                var blog = await _blogService.GetBlogModeration(id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blog moderation successfully", blog);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }          
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetBlogsByFilter([FromQuery] BlogFilterDto filter, bool isFirstRequest)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);

            try
            {
                if (isFirstRequest)
                {
                    var result1 = await _blogService.GetFirstRequestData(userId);
                    return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blogs successfully", result1);
                }
                var result2 = await _blogService.GetBlogsByFilter(filter, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get blogs with filter successfully", result2);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/visibility")]
        public async Task<IActionResult> UpdateVisibility(int id, bool isVisible)
        {
            try
            {
                await _blogService.UpdateBlogVisibility(id, isVisible);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Update blog successfully", $"Blog visibility updated to {(isVisible ? "visible" : "hidden")}." );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("admin/metrics")]
        [Authorize]
        public async Task<IActionResult> GetBlogMetrics([FromQuery] int? communeId,[FromQuery] string? startMonth,[FromQuery] string? endMonth,[FromQuery] int? monthsBack)
        {
            try
            {
                var stats = await _blogService.GetBlogMetricsAsync(communeId, startMonth, endMonth, monthsBack);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Fetched blog metrics.", stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("admin/metrics/officer")]
        [Authorize]
        public async Task<IActionResult> GetBlogMetricsOfficer([FromQuery] string? startMonth,
                                                [FromQuery] string? endMonth,
                                                [FromQuery] int? monthsBack)
        {
            var userId = Guid.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value
            );
            var data = await _blogService.GetBlogMetricsOfficerAsync(userId, startMonth, endMonth, monthsBack);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "OK", data);
        }

    }
}
