using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IConfigurationService
    {
        Task<List<ConfigurationResponseDto>> GetAllAsync();
        Task<ConfigurationResponseDto> GetByIdAsync(int id);
        Task<ConfigurationResponseDto> CreateAsync(ConfigurationCreateDto dto);
        Task<ConfigurationResponseDto> UpdateAsync(ConfigurationUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }


}
