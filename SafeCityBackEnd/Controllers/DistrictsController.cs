using SafeCityBackEnd.Helpers;
using Microsoft.AspNetCore.Mvc;
using BusinessObject.DTOs;
using BusinessObject.DTOs.ResponseModels;
using System.Net;
using Service.Interfaces;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;

[Route("api/communes")]
[ApiExplorerSettings(GroupName = "Communes")]
[ApiController]
public class DistrictsController : ControllerBase
{
    private readonly ICommuneService _districtService;

    public DistrictsController(ICommuneService districtService)
    {
        _districtService = districtService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Officer")]
    public async Task<IActionResult> GetAllCommunes()
    {
        var districts = await _districtService.GetAllAsync();
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get all districts successfully", districts);
    }

    [HttpGet("province/{provinceId}")]
    [Authorize]
    public async Task<IActionResult> GetAllCommunesByProvince(int provinceId)
    {
        try 
        {
            var communes = await _districtService.GetAllForCitizenAsync(provinceId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get all communes by province successfully", communes);
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Officer")]
    public async Task<IActionResult> GetCommuneById(int id)
    {
        var district = await _districtService.GetByIdAsync(id);
        if (district == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "Commune not found", null);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get district successfully", district);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCommune([FromBody] CreateDistrictDTO createDistrictDTO)
    {
        if (createDistrictDTO == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, "Invalid data", null);

        var districtId = await _districtService.CreateAsync(createDistrictDTO);
        var createdDistrict = await _districtService.GetByIdAsync(districtId);

        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Commune created successfully", createdDistrict);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCommune(int id, [FromBody] CreateDistrictDTO districtDTO)
    {
        try
        {
            // Cập nhật district
            await _districtService.UpdateAsync(id, districtDTO);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Commune updated successfully", districtDTO);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCommune(int id)
    {
        var existingDistrict = await _districtService.GetByIdAsync(id);
        if (existingDistrict == null)
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, "Commune not found", null);

        await _districtService.DeleteAsync(id);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Commune deleted successfully", null);
    }



    [HttpPatch("assign-to-officer")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignCommuneToOfficer([FromBody] AssignDistrictToOfficerDTO dto)
    {
        try
        {
            
            var result = await _districtService.AssignDistrictToOfficerAsync(dto.AccountId, dto.DistrictId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Commune assigned to officer successfully", result);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }
    [HttpPatch("unassign-from-officer/{accountId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnassignCommuneFromOfficer(Guid accountId)
    {
        try
        {
            var result = await _districtService.UnassignDistrictFromOfficerAsync(accountId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Officer đã được gỡ phân công quận", result);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SearchCommune([FromQuery] string? name, [FromQuery] int? totalReportedIncidents)
    {
        try
        {

            var districts = await _districtService.SearchAsync(name, totalReportedIncidents);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Search results", districts);
        }
        catch (Exception ex)
        {
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
        }
    }
    [HttpGet("officer/{accountId}/history")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAssignHistory(Guid accountId)
    {
        var history = await _districtService.GetHistoryByAccountIdAsync(accountId);
        return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Lịch sử phân công officer", history);
    }


}
