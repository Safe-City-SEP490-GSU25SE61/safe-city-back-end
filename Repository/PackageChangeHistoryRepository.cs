using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class PackageChangeHistoryRepository : GenericRepository<PackageChangeHistory>, IPackageChangeHistoryRepository
    {
        private readonly AppDbContext _context;

        public PackageChangeHistoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddManyAsync(IEnumerable<PackageChangeHistory> items)
        {
            await _context.PackageChanges.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PackageChangeHistory>> GetByPackageIdAsync(int packageId)
        {
            return await _context.PackageChanges
                .Where(p => p.PackageId == packageId)
                .OrderByDescending(p => p.ChangedAt)
                .ToListAsync();
        }
    }
}
