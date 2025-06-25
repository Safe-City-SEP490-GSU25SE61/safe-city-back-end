using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace SafeCityBackEnd.Controllers
{
    [ApiController]
    [Route("api/change-history")]
    [ApiExplorerSettings(GroupName = "Histories")]
    [Authorize(Roles = "Admin")]
    public class ChangeHistoriesController : ControllerBase
    {
        private readonly IChangeHistoryService _service;

        public ChangeHistoriesController(IChangeHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory([FromQuery] string entityType, [FromQuery] string entityId)
        {
            var result = await _service.GetHistoryByEntityAsync(entityType, entityId);
            return Ok(result);
        }
    }

}
