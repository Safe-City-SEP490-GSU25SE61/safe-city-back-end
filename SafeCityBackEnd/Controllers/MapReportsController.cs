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
    [Route("api/map")]
    [ApiExplorerSettings(GroupName = "Map")]
    public class MapReportsController : ControllerBase
    {
        private readonly IMapService _mapService;
        private readonly ISubscriptionService _subscriptionService;

        public MapReportsController(IMapService mapService, ISubscriptionService subscriptionService)
        {
            _mapService = mapService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet("communes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommunePolygons()
        {
            var result = await _mapService.GetAllCommunePolygonsAsync();
            return Ok(result);
        }

        [HttpGet("reports")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportsForMap([FromQuery] MapReportFilterQuery query)
        {
            try
            {
                var result = await _mapService.GetReportsForMapAsync(
                    query.CommuneId,
                    query.Type?.ToString(),
                    query.Range
                );
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }





        [HttpGet("reports/details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportDetails([FromQuery] MapReportFilterQuery query)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return CustomErrorHandler.SimpleError("User ID claim not found.", 401);

            var userId = Guid.Parse(userIdClaim.Value);
            try
            {
                if (await _subscriptionService.IsSubscribed(userId))
                {
                    var result = await _mapService.GetReportDetailsForMapAsync(
                    query.CommuneId,
                    query.Type?.ToString(),
                    query.Range
                    );
                    return Ok(result);
                }
                return Ok(new { IsPremium = false});
                
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("officer/reports")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportsForOfficer([FromQuery] string? type, [FromQuery] string? range)
        {
            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _mapService.GetOfficerReportsForMapAsync(officerId, type, range);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("officer/reports/details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportDetailsForOfficer([FromQuery] string? type, [FromQuery] string? range)
        {
            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _mapService.GetOfficerReportDetailsForMapAsync(officerId, type, range);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("officer/reports/details/polygon")]
        [AllowAnonymous] //[Authorize(Roles="Officer")]
        public async Task<IActionResult> GetReportDetailsForOfficerPolygon([FromQuery] string? type, [FromQuery] string? range)
        {
            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _mapService.GetOfficerReportDetailsWithPolygonAsync(officerId, type, range);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("admin/reports")]
        [AllowAnonymous] //[Authorize(Roles="Admin")]
        public async Task<IActionResult> GetReportsForAdmin([FromQuery] string? type, [FromQuery] string? range)
        {
            try
            {
                var result = await _mapService.GetAdminReportsForMapAsync(type, range);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("admin/reports/details")]
        [AllowAnonymous] //[Authorize(Roles="Admin")]
        public async Task<IActionResult> GetReportDetailsForAdmin([FromQuery] int communeId, [FromQuery] string? type, [FromQuery] string? range)
        {
            try
            {
                var result = await _mapService.GetAdminReportDetailsAdminAsync(communeId, type, range);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }

}
