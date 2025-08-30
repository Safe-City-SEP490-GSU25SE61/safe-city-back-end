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

        public async Task<Configuration> GetByIdAsync(int id)
        {
            return await _context.Configurations.FindAsync(id);
        }

        public async Task<List<Configuration>> GetAllAsync()
        {
            return await _context.Configurations.ToListAsync();
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
