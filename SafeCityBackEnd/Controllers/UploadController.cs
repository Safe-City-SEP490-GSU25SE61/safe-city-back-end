using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/upload")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Firebase")]
    public class UploadController : ControllerBase
    {
        private readonly IFirebaseStorageService _firebaseService;

        public UploadController(IFirebaseStorageService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var url = await _firebaseService.UploadFileAsync(file, "uploads");
            return Ok(new { url });
        }
    }
}
