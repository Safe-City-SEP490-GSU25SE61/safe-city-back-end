using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IDistrictRepository : IGenericRepository<District>
    {
        Task<District> GetByNameAsync(string name);
        Task<IEnumerable<District>> SearchAsync(string name, int? totalReportedIncidents, int? dangerLevel);
    }

}
