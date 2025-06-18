using SafeCityBackEnd.Helpers;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.DTOs;
using BusinessObject.DTOs.ResponseModels;
using System.Net;
using Service.Interfaces;
using BusinessObject.DTOs.RequestModels;

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
    public async Task<IActionResult> GetAllDistricts()
    {
        var districts = await _districtService.GetAllAsync();
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get all districts successfully", districts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDistrictById(int id)
    {
        var district = await _districtService.GetByIdAsync(id);
        if (district == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "District not found", null);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get district successfully", district);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDistrict([FromBody] CreateDistrictDTO createDistrictDTO)
    {
        if (createDistrictDTO == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid data", null);

        var districtId = await _districtService.CreateAsync(createDistrictDTO);
        var createdDistrict = await _districtService.GetByIdAsync(districtId);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "District created successfully", createdDistrict);
    }

    [HttpPut("{id}")]
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
    public async Task<IActionResult> DeleteDistrict(int id)
    {
        var existingDistrict = await _districtService.GetByIdAsync(id);
        if (existingDistrict == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "District not found", null);

        await _districtService.DeleteAsync(id);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "District deleted successfully", null);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetDistrictByName(string name)
    {
        var district = await _districtService.GetByNameAsync(name);
        if (district == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "District not found", null);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get district by name successfully", district);
    }

    [HttpPatch("assign-to-officer")]
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

}
