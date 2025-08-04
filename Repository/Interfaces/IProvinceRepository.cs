using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IProvinceRepository : IGenericRepository<Province>
    {
        Task<IEnumerable<ProvinceDto>> GetAllProAsync();
    }
}
