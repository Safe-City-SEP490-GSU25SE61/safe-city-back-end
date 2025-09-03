using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IPackageRepository : IGenericRepository<Package>
    {
        Task<Package> GetByIdAsync(int id);  // Lấy thông tin gói dịch vụ theo ID
        Task<IEnumerable<Package>> GetAllAsync(); // Lấy tất cả gói dịch vụ
    }
}
