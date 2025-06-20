using SafeCityBackEnd.Helpers;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs;
using System.Net;
using BusinessObject.DTOs.RequestModels;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

[Route("api/wards")]
[ApiExplorerSettings(GroupName = "Wards")]
[ApiController]
public class WardsController : ControllerBase
{
    private readonly IWardService _wardService;

    public WardsController(IWardService wardService)
    {
        _wardService = wardService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllWards()
    {
        var wards = await _wardService.GetAllAsync();
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get all wards successfully", wards);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetWardById(int id)
    {
        var ward = await _wardService.GetByIdAsync(id);
        if (ward == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "Ward not found", null);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get ward successfully", ward);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateWard([FromBody] CreateWardDTO createWardDTO)
    {
        if (createWardDTO == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid data", null);

        var wardId = await _wardService.CreateAsync(createWardDTO);
        var createdWard = await _wardService.GetByIdAsync(wardId);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Ward created successfully", createdWard);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateWard(int id, [FromBody] CreateWardDTO wardDTO)
    {
        try
        {
            await _wardService.UpdateAsync(id, wardDTO);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Ward updated successfully", wardDTO);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteWard(int id)
    {
        var existingWard = await _wardService.GetByIdAsync(id);
        if (existingWard == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "Ward not found", null);

        await _wardService.DeleteAsync(id);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Ward deleted successfully", null);
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SearchWards([FromQuery] string? name, [FromQuery] int? totalReportedIncidents, [FromQuery] int? dangerLevel, [FromQuery] string? districtName)
    {
        try
        {
            var wards = await _wardService.SearchAsync(name, totalReportedIncidents, dangerLevel, districtName);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Search results", wards);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }
}
