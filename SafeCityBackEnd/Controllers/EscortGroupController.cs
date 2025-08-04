using BusinessObject.DTOs.RequestModels;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service;
using Service.Interfaces;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SafeCityBackEnd.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/escort-groups")]
    [ApiExplorerSettings(GroupName = "Escort Groups")]
    public class EscortGroupController : ControllerBase
    {
        private readonly IEscortJourneyGroupService _groupService;

        public EscortGroupController(IEscortJourneyGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm] CreateEscortJourneyGroupRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                await _groupService.CreateGroupAsync(userId, request);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Group created successfully", null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("group-creation-options")]
        public async Task<IActionResult> GetGroupCreationOptions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            var options = await _groupService.GetAvailableGroupCreationOptionsAsync(userId);

            return Ok(new
            {
                maxOptions = options.Select(o => new { name = o.Item1, value = o.Item2 })
            });
        }



        [HttpPost("join-code")]
        public async Task<IActionResult> JoinGroup(string code)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                await _groupService.JoinGroupAsync(userId, code);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Gửi yêu cầu tham gia nhóm thành công.", null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("review-request")]
        public async Task<IActionResult> ReviewJoinRequest(int requestId, bool approve)
        {
            try
            {
                await _groupService.ReviewJoinRequestAsync(requestId, approve);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, approve ? "Đã chấp nhận yêu cầu." : "Đã từ chối yêu cầu.", null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{groupId}/pending-requests")]
        public async Task<IActionResult> GetPendingRequests(int groupId)
        {
            try
            {
                var requests = await _groupService.GetPendingRequestsByGroupIdAsync(groupId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get data successfully.", requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var groups = await _groupService.GetGroupsByAccountIdAsync(userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get data successfully.", groups);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                await _groupService.DeleteGroupByIdAsync(groupId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Delete successfully.", null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
