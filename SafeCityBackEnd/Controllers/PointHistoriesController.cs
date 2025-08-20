using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service.Interfaces;

namespace SafeCityBackEnd.Controllers
{
    [ApiController]
    [Route("api/points/history")]
    [ApiExplorerSettings(GroupName = "Point History")]
    public class PointHistoriesController : ControllerBase
    {
        private readonly IPointHistoryService _pointHistoryService;

        public PointHistoriesController(IPointHistoryService pointHistoryService)
        {
            _pointHistoryService = pointHistoryService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyHistory(
         [FromQuery] string? range,
         [FromQuery] string? sourceType,
         [FromQuery] bool desc = true) 
        {
            var userIdClaim = User.FindFirst("sub")?.Value
                           ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Unauthorized, "Không xác thực được user.", null);

            try
            {
                var data = await _pointHistoryService.GetHistoryAsync(Guid.Parse(userIdClaim), range, sourceType, desc);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "OK", data);
            }
            catch (ArgumentException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }


        [Authorize(Roles = "Admin,Officer")]
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserHistory(
            Guid userId,
            [FromQuery] string? range,
            [FromQuery] string? sourceType,
            [FromQuery] bool desc = true) 
        {
            try
            {
                var data = await _pointHistoryService.GetHistoryAsync(userId, range, sourceType, desc);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "OK", data);
            }
            catch (ArgumentException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }
    }
}
