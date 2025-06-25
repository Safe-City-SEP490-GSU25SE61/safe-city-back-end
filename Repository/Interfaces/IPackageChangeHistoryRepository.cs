using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IPackageChangeHistoryRepository : IGenericRepository<PackageChangeHistory>
    {
        Task AddManyAsync(IEnumerable<PackageChangeHistory> items);
        Task<IEnumerable<PackageChangeHistory>> GetByPackageIdAsync(int packageId);
    }
}
