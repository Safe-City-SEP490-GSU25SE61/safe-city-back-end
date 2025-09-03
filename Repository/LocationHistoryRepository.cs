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
    public class LocationHistoryRepository : ILocationHistoryRepository
    {
        private readonly AppDbContext _db;

        public LocationHistoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(LocationHistory locationHistory)
        {
            await _db.LocationHistories.AddAsync(locationHistory);
            await _db.SaveChangesAsync();
        }

        public async Task<List<LocationHistory>> GetByJourneyIdAsync(int escortJourneyId)
        {
            return await _db.LocationHistories
                .Where(l => l.EscortJourneyId == escortJourneyId)
                .OrderBy(l => l.RecordedAt)
                .ToListAsync();
        }
    }

}
