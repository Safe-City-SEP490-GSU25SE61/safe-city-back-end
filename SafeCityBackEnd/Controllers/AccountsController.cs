using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interfaces;
using SafeCityBackEnd.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using BusinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/accounts")]
    [ApiExplorerSettings(GroupName = "Accounts")]
    [ApiController]
    public class AccountsController : ODataController
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all accounts")]
        public async Task<IActionResult> GetAccounts()
        {           
            try
            {
                var accounts = await _accountService.GetAllAsync();
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get accounts successfully",
                    accounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{accountId:Guid}")]
        [SwaggerOperation(Summary = "Get account by ID")]
        public async Task<IActionResult> GetExpertise([FromRoute] Guid accountId)
        {
            try
            {
                var account = await _accountService.GetByIdAsync(accountId);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Get account successfully",
                    account);
            } catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Add a new officer account")]
        public async Task<IActionResult> Create([FromForm] AddAccountRequestModel requestModel)
        {
            try
            {
                await _accountService.AddAsync(requestModel);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Add account successfully",
                    "Please inform officer to check their email for account information.");
            } catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("{accountId:Guid}/status")]
        [SwaggerOperation(Summary = "Update account status to 'active' or 'inactive'")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid accountId, [FromBody] UpdateAccountStatusRequestModel requestModel)
        {
            try
            {
                var updated = await _accountService.UpdateStatusAsync(accountId, requestModel);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Account status updated successfully", updated);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
