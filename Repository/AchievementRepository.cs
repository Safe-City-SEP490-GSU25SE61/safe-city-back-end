using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
    {
        private readonly AppDbContext _context;

        public AchievementRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Achievement>> GetAllAsync()
        {
            return await _context.Achievements
                .ToListAsync();
        }

        public async Task<Achievement?> GetByIdAsync(int id)
        {
            return await _context.Achievements.FirstOrDefaultAsync(a => a.Id == id);
        }

      
    }
}
