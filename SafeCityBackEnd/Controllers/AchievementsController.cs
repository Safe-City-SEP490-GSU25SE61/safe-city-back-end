using BusinessObject.DTOs;
using BusinessObject.DTOs.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeCityBackEnd.Helpers;
using Service;
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
        [AllowAnonymous]
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


        [HttpGet]
        [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAchievement([FromBody] AchievementConfigDTO dto)
        {
            try
            {
                var achievementId = await _achievementService.CreateAchievementAsync(dto);
                var createdAchievement = await _achievementService.GetAchievementByIdAsync(achievementId);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.Created, "Achievement created successfully", createdAchievement);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAchievement(int id, [FromBody] AchievementConfigDTOForUpdate dto)
        {
            try
            {

                await _achievementService.UpdateAchievementAsync(id, dto);

                var updatedAchievement = await _achievementService.GetAchievementByIdAsync(id);

                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.OK, "Achievement updated successfully", updatedAchievement);
            }
            catch (Exception ex)
            {
                return CustomSuccessHandler.ResponseBuilder(HttpStatusCode.BadRequest, ex.Message, null);
            }
        }




        [HttpDelete("{achievement_id}")]
        [Authorize(Roles = "Admin")]
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
