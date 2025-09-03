using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAchievementService
    {
        Task<AchievementResponseDTO> GetAchievementByIdAsync(int achievementId);
        Task<IEnumerable<AchievementResponseDTO>> GetAllAchievementsAsync();
        Task<int> CreateAchievementAsync(AchievementConfigDTO dto, IFormFile? image = null);
        Task UpdateAchievementAsync(int achievementId, AchievementConfigDTOForUpdate dto, IFormFile? image = null);
        Task DeleteAchievementAsync(int achievementId); 
    }
}
