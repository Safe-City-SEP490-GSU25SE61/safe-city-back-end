using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICommuneRepository : IGenericRepository<Commune>
    {
        Task<Commune> GetByNameAsync(string name);
        Task<IEnumerable<CommuneForCitizenDTO>> GetAllActiveByProvinceAsync(int provinceId);
        Task<IEnumerable<Commune>> SearchAsync(string name, int? totalReportedIncidents);
    }

}
