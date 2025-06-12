using BusinessObject.DTOs;
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
        Task CreateAsync(DistrictDTO districtDTO);
        Task UpdateAsync(DistrictDTO districtDTO);
        Task DeleteAsync(int id);
        Task<DistrictDTO> GetByNameAsync(string name);
    }

}
