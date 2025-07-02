using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service.Interfaces;
using System.Net;
using System.Security.Claims;

namespace SafeCityBackEnd.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [ApiExplorerSettings(GroupName = "Incident Reports")]
    public class IncidentReportsController : ControllerBase
    {
        private readonly IIncidentReportService _reportService;

        public IncidentReportsController(IIncidentReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReport([FromForm] CreateReportRequestModel model)
        {
            if (!ModelState.IsValid)
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid report data", null);

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var createdReport = await _reportService.CreateAsync(model, userId);

            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Report created successfully", createdReport);
        }



        [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            try
            {
                var report = await _reportService.GetByIdAsync(id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Report retrieved successfully", report);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _reportService.GetAllAsync();
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Retrieved all reports successfully", reports);
        }

        [HttpPatch("{id:Guid}/status")]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<IActionResult> UpdateReportStatus(Guid id, [FromBody] UpdateReportStatusRequestModel model)
        {
            if (!ModelState.IsValid)
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid status data", null);

            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var updated = await _reportService.UpdateStatusAsync(id, model, officerId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Report status updated successfully", updated);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }

        [HttpPost("{id:Guid}/note")]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<IActionResult> AddInternalNote(Guid id, [FromBody] AddInternalNoteRequestModel model)
        {
            if (!ModelState.IsValid)
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid note data", null);

            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var updated = await _reportService.AddNoteAsync(id, model, officerId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Internal note added successfully", updated);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }

    }

}
