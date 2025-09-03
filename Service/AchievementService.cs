using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
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
        private readonly IFirebaseStorageService _firebaseStorage;
        private static readonly string[] AllowedImageExt = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] AllowedImageMime = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxImageBytes = 10 * 1024 * 1024;

        public AchievementService(IAchievementRepository achievementRepository, IFirebaseStorageService firebaseStorage)
        {
            _achievementRepository = achievementRepository;
            _firebaseStorage = firebaseStorage;
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
                LastUpdated = DateTimeHelper.ToVietnamTime(achievement.LastUpdated),
                ImageUrl = achievement.ImageUrl
            };
        }

        public async Task<int> CreateAchievementAsync(AchievementConfigDTO dto, IFormFile? image = null)
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

            if (image != null)
            {
                ValidateImage(image); 
                achievement.ImageUrl = await _firebaseStorage.UploadFileAsync(image, "achievements");
            }

            await _achievementRepository.CreateAsync(achievement);

            return achievement.Id;
        }

        public async Task UpdateAchievementAsync(int achievementId, AchievementConfigDTOForUpdate dto, IFormFile? image = null)
        {
            var achievement = await _achievementRepository.GetByIdAsync(achievementId);
            if (achievement == null)
                throw new KeyNotFoundException("Danh hiệu không tìm thấy.");

            achievement.Description = dto.Description;
            achievement.MinPoint = dto.MinPoint;
            achievement.Benefit = dto.Benefit;
            achievement.LastUpdated = DateTime.UtcNow;

            if (image != null) 
            {
                ValidateImage(image);
                achievement.ImageUrl = await _firebaseStorage.UploadFileAsync(image, "achievements");
            }

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
                LastUpdated = DateTimeHelper.ToVietnamTime(achievement.LastUpdated),
                ImageUrl = achievement.ImageUrl
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
        private void ValidateImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new InvalidOperationException("Ảnh tải lên bị trống.");

            var ext = Path.GetExtension(image.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedImageExt.Contains(ext, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Định dạng ảnh không hợp lệ. Chỉ hỗ trợ JPG/PNG/WEBP.");

            if (!AllowedImageMime.Contains(image.ContentType, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("MIME type ảnh không hợp lệ.");

            if (image.Length > MaxImageBytes)
                throw new InvalidOperationException("Ảnh vượt quá dung lượng tối đa 5MB.");
        }

    }
}
