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

            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var createdReport = await _reportService.CreateAsync(model, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Report created successfully", createdReport);
            }
            catch (InvalidOperationException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.InternalServerError, "Lỗi hệ thống", null);
            }
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
        [HttpPatch("{id:Guid}/cancel")]
        [Authorize]//(Roles = "Citizen")]
        public async Task<IActionResult> CancelReport(Guid id, [FromBody] CancelReportRequestModel model)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var cancelled = await _reportService.CancelAsync(id, userId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Report cancelled successfully", cancelled);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
            catch (InvalidOperationException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }
        [HttpGet("officer")]
        [Authorize]
        public async Task<IActionResult> GetReportsByDistrict()
        {
            var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var reports = await _reportService.GetReportsByOfficerDistrictAsync(officerId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Reports by officer district", reports);
        }

        [HttpGet("officer/filter")]
        [Authorize]
        public async Task<IActionResult> GetFilteredReportsByOfficer([FromQuery] string? range, [FromQuery] string? status,[FromQuery] string? ward, [FromQuery] bool includeRelated = false)
        {
            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var reports = await _reportService.GetFilteredReportsByOfficerAsync(officerId, range, status, ward);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Filtered reports", reports);
            }
            catch (ArgumentException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
            catch (InvalidOperationException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Forbidden, ex.Message, null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.InternalServerError, "Đã xảy ra lỗi không xác định.", null);
            }
        }
        [HttpGet("citizen/filter")]
        [Authorize]// (Roles = "Citizen")]
        public async Task<IActionResult> GetFilteredReportsByCitizen([FromQuery] string? range, [FromQuery] string? status)
        {
            try
            {
                var citizenId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var reports = await _reportService.GetFilteredReportsByCitizenAsync(citizenId, range, status);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Filtered reports by citizen", reports);
            }
            catch (ArgumentException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpPatch("{id:Guid}/transfer")]
        [Authorize]
        public async Task<IActionResult> TransferDistrict(Guid id, [FromBody] TransferReportDistrictRequestModel model)
        {
            try
            {
                var officerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _reportService.TransferDistrictAsync(id, model, officerId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Chuyển khu vực thành công", result);
            }
            catch (KeyNotFoundException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
            catch (InvalidOperationException ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }




    }

}
