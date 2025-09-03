using BusinessObject.DTOs.RequestModels;
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

        public VirtualEscortController(IVirtualEscortService virtualEscortService)
        {
            _virtualEscortService = virtualEscortService;
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
    }
}
