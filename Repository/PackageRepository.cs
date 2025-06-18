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
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly AppDbContext _context;

        public PackageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        
        public async Task<Package> GetByIdAsync(int id)
        {
            return await _context.Packages.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Package>> GetAllAsync()
        {
            return await _context.Packages.ToListAsync();
        }
    }
}

