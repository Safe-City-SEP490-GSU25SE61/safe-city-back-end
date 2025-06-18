using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achievementRepository;

        public AchievementService(IAchievementRepository achievementRepository)
        {
            _achievementRepository = achievementRepository;
        }
        public async Task<AchievementResponseDTO> GetAchievementByIdAsync(int achievementId)
        {
            var achievement = await _achievementRepository.GetByIdAsync(achievementId);
            if (achievement == null)
            {
                throw new KeyNotFoundException("Achievement not found.");
            }

            return new AchievementResponseDTO
            {
                Id = achievement.Id,
                Name = achievement.Name,
                Description = achievement.Description,
                MinPoint = achievement.MinPoint,
                Benefit = achievement.Benefit
            };
        }


        public async Task<IEnumerable<AchievementResponseDTO>> GetAllAchievementsAsync()
        {
            var achievements = await _achievementRepository.GetAllAsync();
            return achievements.Select(achievement => new AchievementResponseDTO
            {
                Id = achievement.Id,
                Name = achievement.Name,
                Description = achievement.Description,
                MinPoint = achievement.MinPoint,
                Benefit = achievement.Benefit
            }).ToList();
        }

        public async Task CreateAchievementAsync(AchievementConfigDTO dto)
        {
            var achievement = new Achievement
            {
                Name = dto.Name,
                Description = dto.Description,
                MinPoint = dto.MinPoint,
                Benefit = dto.Benefit
            };

            await _achievementRepository.CreateAsync(achievement);
        }

        public async Task UpdateAchievementAsync(int achievementId, AchievementConfigDTOForUpdate dto)
        {
            var achievement = await _achievementRepository.GetByIdAsync(achievementId);
            if (achievement == null)
            {
                throw new KeyNotFoundException("Achievement not found.");
            }

            achievement.Description = dto.Description;
            achievement.MinPoint = dto.MinPoint;
            achievement.Benefit = dto.Benefit;

            await _achievementRepository.UpdateAsync(achievement);
        }

        public async Task DeleteAchievementAsync(int achievementId)
        {
            var achievement = await _achievementRepository.GetByIdAsync(achievementId);
            if (achievement == null)
            {
                throw new KeyNotFoundException("Achievement not found.");
            }

            await _achievementRepository.DeleteAsync(achievementId);
        }
    }
}
