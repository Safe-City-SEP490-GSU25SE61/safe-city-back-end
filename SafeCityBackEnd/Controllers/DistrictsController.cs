using BusinessObject.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictsController : ControllerBase
    {
        private readonly IDistrictService _districtService;

        public DistrictsController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistrictDTO>>> GetAllDistricts()
        {
            var districts = await _districtService.GetAllAsync();
            return Ok(districts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DistrictDTO>> GetDistrictById(int id)
        {
            var district = await _districtService.GetByIdAsync(id);
            if (district == null)
                return NotFound();
            return Ok(district);
        }

        [HttpPost]
        public async Task<ActionResult> CreateDistrict([FromBody] DistrictDTO districtDTO)
        {
            if (districtDTO == null)
                return BadRequest();

            await _districtService.CreateAsync(districtDTO);
            return CreatedAtAction(nameof(GetDistrictById), new { id = districtDTO.Id }, districtDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDistrict(int id, [FromBody] DistrictDTO districtDTO)
        {
            var existingDistrict = await _districtService.GetByIdAsync(id);
            if (existingDistrict == null)
                return NotFound();

            await _districtService.UpdateAsync(districtDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDistrict(int id)
        {
            var existingDistrict = await _districtService.GetByIdAsync(id);
            if (existingDistrict == null)
                return NotFound();

            await _districtService.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("name/{name}")]
        public async Task<ActionResult<DistrictDTO>> GetDistrictByName(string name)
        {
            var district = await _districtService.GetByNameAsync(name);
            if (district == null)
                return NotFound();
            return Ok(district);
        }
    }

}
