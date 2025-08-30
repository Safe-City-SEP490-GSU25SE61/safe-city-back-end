using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IConfigurationRepository
    {
        Task<Configuration> GetByIdAsync(int id);
        Task<List<Configuration>> GetAllAsync();
        Task AddAsync(Configuration config);
        Task UpdateAsync(Configuration config);
        Task DeleteAsync(Configuration config);
        Task<bool> ExistsAsync(int id);
    }


}
