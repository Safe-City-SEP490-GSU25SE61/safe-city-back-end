using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IAchievementRepository : IGenericRepository<Achievement>
    {
        Task<Achievement?> GetByIdAsync(int id);  
        Task<IEnumerable<Achievement>> GetAllAsync(); 
    }
}
