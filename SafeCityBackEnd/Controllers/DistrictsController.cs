using SafeCityBackEnd.Helpers;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.DTOs;
using BusinessObject.DTOs.ResponseModels;
using System.Net;
using Service.Interfaces;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;

[Route("api/districts")]
[ApiExplorerSettings(GroupName = "Districts")]
[ApiController]
public class DistrictsController : ControllerBase
{
    private readonly IDistrictService _districtService;

    public DistrictsController(IDistrictService districtService)
    {
        _districtService = districtService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllDistricts()
    {
        var districts = await _districtService.GetAllAsync();
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get all districts successfully", districts);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDistrictById(int id)
    {
        var district = await _districtService.GetByIdAsync(id);
        if (district == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "District not found", null);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get district successfully", district);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDistrict([FromBody] CreateDistrictDTO createDistrictDTO)
    {
        if (createDistrictDTO == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid data", null);

        var districtId = await _districtService.CreateAsync(createDistrictDTO);
        var createdDistrict = await _districtService.GetByIdAsync(districtId);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "District created successfully", createdDistrict);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDistrict(int id, [FromBody] CreateDistrictDTO districtDTO)
    {
        try
        {
            // Cập nhật district
            await _districtService.UpdateAsync(id, districtDTO);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "District updated successfully", districtDTO);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDistrict(int id)
    {
        var existingDistrict = await _districtService.GetByIdAsync(id);
        if (existingDistrict == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "District not found", null);

        await _districtService.DeleteAsync(id);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "District deleted successfully", null);
    }



    [HttpPatch("assign-to-officer")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignDistrictToOfficer([FromBody] AssignDistrictToOfficerDTO dto)
    {
        try
        {
            
            var result = await _districtService.AssignDistrictToOfficerAsync(dto.AccountId, dto.DistrictId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "District assigned to officer successfully", result);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }
    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SearchDistricts([FromQuery] string? name, [FromQuery] int? totalReportedIncidents, [FromQuery] int? dangerLevel)
    {
        try
        {

            var districts = await _districtService.SearchAsync(name, totalReportedIncidents, dangerLevel);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Search results", districts);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }

}
