using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service.Interfaces;
using System.Net;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/packages")]
    [ApiExplorerSettings(GroupName = "Packages")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync();
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Packages retrieved successfully.", packages);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpGet("{packageId}")]
        public async Task<IActionResult> GetPackageById(int packageId)
        {
            try
            {
                var package = await _packageService.GetPackageByIdAsync(packageId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Package retrieved successfully.", package);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDTO dto)
        {
            try
            {
                await _packageService.CreatePackageAsync(dto);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Package created successfully.", null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpPut("{packageId}")]
        public async Task<IActionResult> UpdatePackage(int packageId, [FromBody] UpdatePackageDTO dto)
        {
            try
            {
                await _packageService.UpdatePackageAsync(packageId, dto);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Package updated successfully.", null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpDelete("{packageId}")]
        public async Task<IActionResult> DeletePackage(int packageId)
        {
            try
            {
                await _packageService.DeletePackageAsync(packageId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Package deactivated successfully.", null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }
    }

}
