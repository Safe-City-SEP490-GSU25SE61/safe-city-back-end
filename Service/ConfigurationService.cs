using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using MediatR;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _repository;
        private readonly IFirebaseStorageService _firebaseStorageService;

        public ConfigurationService(IConfigurationRepository repository, IFirebaseStorageService firebaseStorageService)
        {
            _repository = repository;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<List<ConfigurationResponseDto>> GetAllAsync(string? keyword)
        {
            var configs = await _repository.GetAllAsync(keyword);
            return configs.Select(c => new ConfigurationResponseDto
            {
                Id = c.Id,
                Category = c.Category,
                Key = c.Key,
                Value = c.Value,
                Description = c.Description
            }).ToList();
        }

        public async Task<ConfigurationResponseDto> GetByIdAsync(int id)
        {
            var config = await _repository.GetByIdAsync(id);
            if (config == null) return null;

            return new ConfigurationResponseDto
            {
                Id = config.Id,
                Category = config.Category,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description
            };
        }

        public async Task<ConfigurationResponseDto> CreateAsync(ConfigurationCreateDto dto)
        {
            var image = (dto.MediaFile != null && dto.MediaFile.ContentType?.StartsWith("image") == true)
                ? dto.MediaFile
                : null;

            var config = new Configuration
            {
                Category = dto.Category,
                Key = dto.Key,
                Description = dto.Description
            };

            if (image != null)
            {
                var url = await _firebaseStorageService.UploadFileAsync(image, "uploads");

                config.Value = url;
            }
            else
            {
                config.Value = string.IsNullOrWhiteSpace(dto.Value) ? null : dto.Value.Trim();
            }

            await _repository.AddAsync(config);

            return new ConfigurationResponseDto
            {
                Id = config.Id,
                Category = config.Category,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description
            };
        }

        public async Task<ConfigurationResponseDto> UpdateAsync(ConfigurationUpdateDto dto)
        {
            var config = await _repository.GetByIdAsync(dto.Id);
            if (config == null) return null;

            var image = (dto.MediaFile != null && dto.MediaFile.ContentType?.StartsWith("image") == true)
                ? dto.MediaFile
                : null;

            if (image != null)
            {
                var url = await _firebaseStorageService.UploadFileAsync(image, "uploads");

                config.Value = url;
            }
            else
            {
                config.Value = string.IsNullOrWhiteSpace(dto.Value) ? null : dto.Value.Trim();
            }

            config.Category = dto.Category;
            config.Key = dto.Key;
            config.Description = dto.Description;

            await _repository.UpdateAsync(config);

            return new ConfigurationResponseDto
            {
                Id = config.Id,
                Category = config.Category,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var config = await _repository.GetByIdAsync(id);
            if (config == null) return false;

            await _repository.DeleteAsync(config);
            return true;
        }
    }


}
