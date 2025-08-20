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
    public class JourneyRepository : IJourneyRepository
    {
        private readonly AppDbContext _context;

        public JourneyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EscortJourney> AddAsync(EscortJourney journey)
        {
            await _context.EscortJourneys.AddAsync(journey);
            await _context.SaveChangesAsync();
            return journey;
        }

        public async Task<EscortJourney> GetByUserIdAsync(Guid userId)
        {
            var escort = await _context.EscortJourneys
                        .Include(e => e.User)
                        .Include(e => e.Watchers)
                        .FirstOrDefaultAsync(e => e.UserId == userId);

            return escort;
        }
    }

}
