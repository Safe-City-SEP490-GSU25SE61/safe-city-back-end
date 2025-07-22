using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Helpers;
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
                Benefit = achievement.Benefit,
                CreateAt = DateTimeHelper.ToVietnamTime(achievement.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(achievement.LastUpdated)
            };
        }

        public async Task<int> CreateAchievementAsync(AchievementConfigDTO dto)
        {
            var achievement = new Achievement
            {
                Name = dto.Name,
                Description = dto.Description,
                MinPoint = dto.MinPoint,
                Benefit = dto.Benefit,
                CreateAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _achievementRepository.CreateAsync(achievement);

            return achievement.Id;
        }

        public async Task UpdateAchievementAsync(int achievementId, AchievementConfigDTOForUpdate dto)
        {
            var achievement = await _achievementRepository.GetByIdAsync(achievementId);
            if (achievement == null)
                throw new KeyNotFoundException("Danh hiệu không tìm thấy.");

            achievement.Description = dto.Description;
            achievement.MinPoint = dto.MinPoint;
            achievement.Benefit = dto.Benefit;
            achievement.LastUpdated = DateTime.UtcNow;

            await _achievementRepository.UpdateAsync(achievement);
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
                Benefit = achievement.Benefit,
                CreateAt = DateTimeHelper.ToVietnamTime(achievement.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(achievement.LastUpdated)
            }).ToList();
        }

        public async Task DeleteAchievementAsync(int achievementId)
        {
            var achievement = await _achievementRepository.GetByIdAsync(achievementId);
            if (achievement == null)
            {
                throw new KeyNotFoundException("Danh hiệu không tìm thấy.");
            }

            await _achievementRepository.DeleteAsync(achievementId);
        }

    }
}
