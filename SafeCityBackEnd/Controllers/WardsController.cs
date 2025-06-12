using BusinessObject.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WardsController : ControllerBase
    {
        private readonly IWardService _wardService;

        public WardsController(IWardService wardService)
        {
            _wardService = wardService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WardDTO>>> GetAllWards()
        {
            var wards = await _wardService.GetAllAsync();
            return Ok(wards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WardDTO>> GetWardById(int id)
        {
            var ward = await _wardService.GetByIdAsync(id);
            if (ward == null)
                return NotFound();
            return Ok(ward);
        }

        [HttpPost]
        public async Task<ActionResult> CreateWard([FromBody] WardDTO wardDTO)
        {
            if (wardDTO == null)
                return BadRequest();

            await _wardService.CreateAsync(wardDTO);
            return CreatedAtAction(nameof(GetWardById), new { id = wardDTO.Id }, wardDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWard(int id, [FromBody] WardDTO wardDTO)
        {
            var existingWard = await _wardService.GetByIdAsync(id);
            if (existingWard == null)
                return NotFound();

            await _wardService.UpdateAsync(wardDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWard(int id)
        {
            var existingWard = await _wardService.GetByIdAsync(id);
            if (existingWard == null)
                return NotFound();

            await _wardService.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("name/{name}")]
        public async Task<ActionResult<WardDTO>> GetWardByName(string name)
        {
            var ward = await _wardService.GetByNameAsync(name);
            if (ward == null)
                return NotFound();
            return Ok(ward);
        }
    }

}
