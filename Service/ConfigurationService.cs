using BusinessObject.DTOs.RequestModels;
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
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _repository;

        public ConfigurationService(IConfigurationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ConfigurationResponseDto>> GetAllAsync()
        {
            var configs = await _repository.GetAllAsync();
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
            var config = new Configuration
            {
                Category = dto.Category,
                Key = dto.Key,
                Value = dto.Value,
                Description = dto.Description
            };

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

            config.Category = dto.Category;
            config.Key = dto.Key;
            config.Value = dto.Value;
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
