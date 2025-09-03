using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceResponseDTO>> GetAllAsync();
        Task<Province> GetByIdAsync(int id);
        Task CreateAsync(CreateProvinceDTO province);
        Task UpdateAsync(Province province);
        Task DeleteAsync(int id);
    }

}
