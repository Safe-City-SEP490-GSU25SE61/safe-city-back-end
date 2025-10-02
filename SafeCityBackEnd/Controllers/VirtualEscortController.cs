using AutoMapper.Execution;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service.Interfaces;
using System.Security.Claims;

namespace SafeCityBackEnd.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/virtual-escorts")]
    [ApiExplorerSettings(GroupName = "Virtual Escorts")]
    public class VirtualEscortController : ControllerBase
    {
        private readonly IVirtualEscortService _virtualEscortService;
        private readonly ISosAlertService _sosService;

        public VirtualEscortController(IVirtualEscortService virtualEscortService, ISosAlertService sosAlertService)
        {
            _virtualEscortService = virtualEscortService;
            _sosService = sosAlertService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateJourney([FromForm] CreateJourneyDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var journey = await _virtualEscortService.CreateJourneyFromGoongResponseAsync(userId, request);
                return Ok(new { journey });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected error", error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetJourneyHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var history = await _virtualEscortService.GetJourneyHistoryAsync(userId);
                if (history == null || history.EscortGroupDtos.Count == 0)
                    return NotFound("No journeys found for this user");

                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected error", error = ex.Message });
            }
        }

        [HttpGet("journey-for-observer")]
        public async Task<IActionResult> GetJourneyForObserver(int memberId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var routeJson = await _virtualEscortService.GetJourneyForObserverAsync(userId, memberId);

                return Ok(routeJson);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected error", error = ex.Message });
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSos([FromBody] SosCreateRequest req)
        {
            if (req == null) return BadRequest("Invalid request");

            var senderToken = await _sosService.CreateAlertAsync(
                req.EscortJourneyId,
                req.SenderId,
                req.Lat,
                req.Lng,
                DateTime.UtcNow
            );

            return Ok(new
            {
                ChannelName = $"sos_{req.EscortJourneyId}",
                SenderName = senderToken.senderName,
                SenderToken = senderToken.token,
                AlertId = senderToken.alertId,
            });
        }


        [HttpPost("{sosAlertId}/end")]
        public async Task<IActionResult> EndSos(int sosAlertId)
        {
            await _sosService.EndSosCallAsync(sosAlertId);
            return Ok(new { Message = "SOS call ended" });
        }

        [HttpPost("{sosAlertId}/watchers/join")]
        public async Task<IActionResult> JoinWatcher(int sosAlertId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                var result = await _sosService.JoinWatcherAsync(sosAlertId, userId);
                return Ok(new
                {
                    ChannelName = result.channelName,
                    Token = result.token,
                    UID = result.uid,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected error", error = ex.Message });
            }
        }

        [HttpPost("{sosAlertId}/watchers/leave")]
        public async Task<IActionResult> LeaveWatcher(int sosAlertId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            await _sosService.LeaveWatcherAsync(sosAlertId, userId);
            return Ok(new { Message = "Watcher left" });
        }
    }
}
