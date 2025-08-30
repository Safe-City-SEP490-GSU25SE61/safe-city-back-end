using BusinessObject.DTOs.ResponseModels;
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

        public async Task<EscortJourney> GetActiveJourneyByGroupMemberIdAsync(int memberId)
        {
            var escort = await _context.EscortJourneys
                        .Include(e => e.User)
                        .Include(e => e.Watchers)
                        .FirstOrDefaultAsync(e => e.MemberId == memberId && e.Status.Equals("Active"));

            return escort;
        }

        public async Task<List<EscortJourneyDto>> GetJourneysByUserIdAsync(Guid userId)
        {
            return await _context.EscortJourneys
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.StartTime)
                .Select(j => new EscortJourneyDto
                {
                    Id = j.Id,
                    StartLocation = j.StartPoint,
                    EndLocation = j.EndPoint,
                    StartTime = j.StartTime != null ? j.StartTime.Value : null,
                    EndTime = j.ArrivalTime != null ? j.ArrivalTime.Value : null,
                    Vehicle = j.Vehicle,
                    Status = j.Status,
                    Watchers = j.Watchers.Select(w => new WatcherDto
                    {
                        MemberId = w.WatcherId,
                        FullName = w.Watcher.Account.FullName
                    }).ToList()
                })
                .ToListAsync();
        }

    }

}
