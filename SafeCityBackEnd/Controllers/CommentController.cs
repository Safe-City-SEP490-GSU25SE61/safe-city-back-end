using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using BusinessObject.DTOs.RequestModels;
using SafeCityBackEnd.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/comments")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Comment")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                await _commentService.AddCommentAsync(userId, dto);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Write comment successfully.", "ok");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{blogId}")]
        public async Task<IActionResult> GetCommentsForBlog(int blogId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByBlogIdAsync(blogId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[HttpGet("post/{postId}")]
        //public async Task<IActionResult> GetComments(int postId)
        //{
        //    try
        //    {
        //        var result = await _commentService.GetCommentsByPostIdAsync(postId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
    }
}
