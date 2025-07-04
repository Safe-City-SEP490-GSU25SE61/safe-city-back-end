using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IIncidentReportRepository
    {
        Task<IncidentReport?> GetByIdAsync(Guid id);
        Task<IEnumerable<IncidentReport>> GetAllAsync();
        Task CreateAsync(IncidentReport report);
        Task UpdateStatusAsync(Guid id, string status, Guid officerId);
    }

}
