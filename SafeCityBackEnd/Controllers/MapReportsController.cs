using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

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
        public async Task<IActionResult> GetReportsForMap([FromQuery] int communeId)
        {
            var result = await _mapService.GetReportsForMapAsync(communeId);
            return Ok(result);
        }




        [HttpGet("reports/details")]
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

    }

}
