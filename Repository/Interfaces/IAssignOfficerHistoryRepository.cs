using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAssignOfficerHistoryRepository : IGenericRepository<AssignOfficerHistory>
    {
        Task<IEnumerable<AssignOfficerHistory>> GetByAccountIdAsync(Guid accountId);
    }
}
