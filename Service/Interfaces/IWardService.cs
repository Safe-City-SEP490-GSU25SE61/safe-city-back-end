using BusinessObject.DTOs;
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
    public interface IWardService
    {
        Task<IEnumerable<WardDTO>> GetAllAsync();
        Task<WardDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateWardDTO wardDTO);
        Task UpdateAsync(int id, CreateWardDTO wardDTO);
        Task DeleteAsync(int id);
        Task<WardDTO> GetByNameAsync(string name);
    }

}
