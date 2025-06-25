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
    public class ChangeHistoryRepository : GenericRepository<ChangeHistory>, IChangeHistoryRepository
    {
        private readonly AppDbContext _context;

        public ChangeHistoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChangeHistory>> GetByEntityAsync(string entityType, string entityId)
        {
            return await _context.ChangeHistories
                .Where(h => h.EntityType == entityType && h.EntityId == entityId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }

}
