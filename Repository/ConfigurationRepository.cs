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
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly AppDbContext _context;

        public ConfigurationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Configuration?> GetByIdAsync(int id)
        {
            return await _context.Configurations.FindAsync(id);
        }

        public async Task<Configuration?> GetByKeyNameAsync(string keyName)
        {
            return await _context.Configurations.FirstOrDefaultAsync(c => c.Key.ToLower().Equals(keyName.ToLower()));
        }

        public async Task<List<Configuration>> GetAllAsync(string? keyword)
        {
            var query = _context.Configurations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(c =>
                    c.Key.ToLower().Contains(keyword) ||
                    c.Category.ToLower().Contains(keyword));
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Configuration config)
        {
            _context.Configurations.Add(config);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Configuration config)
        {
            _context.Configurations.Update(config);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Configuration config)
        {
            _context.Configurations.Remove(config);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Configurations.AnyAsync(c => c.Id == id);
        }
    }


}
