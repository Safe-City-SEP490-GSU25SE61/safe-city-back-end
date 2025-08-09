using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public MapReportsController(IMapService mapService)
        {
            _mapService = mapService;
        }

        [HttpGet("communes")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetCommunePolygons()
        {
            var result = await _mapService.GetAllCommunePolygonsAsync();
            return Ok(result);
        }

        [HttpGet("reports")]
        //[AllowAnonymous]
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
        //[AllowAnonymous]
        public async Task<IActionResult> GetReportDetails([FromQuery] MapReportFilterQuery query)
        {
            try
            {
                var result = await _mapService.GetReportDetailsForMapAsync(
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
        [HttpGet("officer/reports")]
        //[Authorize]
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
        //[Authorize]
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


    }

}
