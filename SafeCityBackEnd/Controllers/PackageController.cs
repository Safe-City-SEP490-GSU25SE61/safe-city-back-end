using BusinessObject.DTOs;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync();
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Lấy tất cả gói dịch vụ thành công.", packages);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }


        [HttpGet("{packageId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPackageById(int packageId)
        {
            try
            {
                var package = await _packageService.GetPackageByIdAsync(packageId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Lấy gói dịch vụ thành công.", package);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDTO dto)
        {
            try
            {
                var packageId = await _packageService.CreatePackageAsync(dto);
                var createdPackage = await _packageService.GetPackageByIdAsync(packageId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Package created successfully", createdPackage);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageDTO dto)
        {
            try
            {
                await _packageService.UpdatePackageAsync(id, dto);

                var updatedPackage = await _packageService.GetPackageByIdAsync(id);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Package updated successfully", updatedPackage);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }



        [HttpDelete("{packageId}")]
        [Authorize(Roles = "Admin")]
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
        [HttpGet("{packageId}/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPackageChangeHistory(int packageId)
        {
            var history = await _packageService.GetHistoryByIdAsync(packageId);
            return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Lịch sử thay đổi gói", history);
        }

    }

}
