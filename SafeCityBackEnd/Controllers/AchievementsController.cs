using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service.Interfaces;
using System.Net;

namespace SafeCityBackEnd.Controllers
{
    [Route("api/achievement/config")]
    [ApiExplorerSettings(GroupName = "Achievements")]
    [ApiController]
    public class AchievementsController : ControllerBase
    {
        private readonly IAchievementService _achievementService;

        public AchievementsController(IAchievementService achievementService)
        {
            _achievementService = achievementService;
        }
        [HttpGet("{achievement_id}")]
        public async Task<IActionResult> GetAchievementById(int achievement_id)
        {
            try
            {
                var achievement = await _achievementService.GetAchievementByIdAsync(achievement_id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Achievement retrieved successfully.", achievement);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.NotFound, ex.Message, null);
            }
        }

        // Lấy tất cả tiêu chí cấp bậc (GET)
        [HttpGet]
        public async Task<IActionResult> GetAllAchievements()
        {
            try
            {
                var achievements = await _achievementService.GetAllAchievementsAsync();
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Achievements retrieved successfully.", achievements);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAchievement([FromBody] AchievementConfigDTO dto)
        {
            try
            {
                await _achievementService.CreateAchievementAsync(dto);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Achievement created successfully.", null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpPatch("{achievement_id}")]
        public async Task<IActionResult> UpdateAchievement(int achievement_id, [FromBody] AchievementConfigDTOForUpdate dto)
        {
            try
            {
                await _achievementService.UpdateAchievementAsync(achievement_id, dto);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Achievement updated successfully.", null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpDelete("{achievement_id}")]
        public async Task<IActionResult> DeleteAchievement(int achievement_id)
        {
            try
            {
                await _achievementService.DeleteAchievementAsync(achievement_id);
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Achievement deleted successfully.", null);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }
    }

}
