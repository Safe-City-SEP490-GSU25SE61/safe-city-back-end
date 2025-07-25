using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/provinces")]
    [ApiExplorerSettings(GroupName = "Provinces")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _provinceService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var province = await _provinceService.GetByIdAsync(id);
            if (province == null)
                return NotFound();

            return Ok(province);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProvinceDTO province)
        {
            try
            {
                await _provinceService.CreateAsync(province);
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Province province)
        {
            try
            {
                await _provinceService.UpdateAsync(province);
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _provinceService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) { return BadRequest(ex.Message);
            }   
        }
    }
}
