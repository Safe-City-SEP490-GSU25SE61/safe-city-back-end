using BusinessObject.DTOs;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IDistrictService
    {
        Task<IEnumerable<DistrictDTO>> GetAllAsync();
        Task<DistrictDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateDistrictDTO createDistrictDTO);
        Task UpdateAsync(int id, CreateDistrictDTO districtDTO);
        Task DeleteAsync(int id);
        Task<DistrictDTO> GetByNameAsync(string name);
        Task<bool> AssignDistrictToOfficerAsync(Guid accountId, int districtId);
    }

}
