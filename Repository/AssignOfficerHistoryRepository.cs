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
    public class AssignOfficerHistoryRepository : GenericRepository<AssignOfficerHistory>, IAssignOfficerHistoryRepository
    {
        private readonly AppDbContext _context;

        public AssignOfficerHistoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssignOfficerHistory>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.AssignOffers
                .Where(h => h.AccountId == accountId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
