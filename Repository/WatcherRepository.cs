using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class WatcherRepository : IWatcherRepository
    {
        private readonly AppDbContext _db;
        public WatcherRepository(AppDbContext db) 
        {
            _db = db;
        }

        public async Task<List<EscortJourneyWatcher>> GetWatchersByJourneyIdAsync(int escortJourneyId)
        {
            return await _db.EscortJourneyWatchers
                .Where(w => w.EscortJourneyId == escortJourneyId)
                .ToListAsync();
        }

        public async Task<List<EscortJourneyWatcher>> GetWatchersBySosAlertAsync(int sosAlertId, int? escortJourneyId = null)
        {
            if (escortJourneyId.HasValue)
            {
                var sos = await _db.SosAlerts.FindAsync(sosAlertId);
                if (sos == null) return new List<EscortJourneyWatcher>();
                return await _db.EscortJourneyWatchers
                    .Where(w => w.EscortJourneyId == escortJourneyId.Value)
                    .ToListAsync();
            }

            return new List<EscortJourneyWatcher>();
        }

        public async Task UpdateAsync(EscortJourneyWatcher watcher)
        {
            _db.EscortJourneyWatchers.Update(watcher);
            await _db.SaveChangesAsync();
        }

        public async Task<EscortJourneyWatcher?> GetByIdAsync(int id)
        {
            return await _db.EscortJourneyWatchers.FindAsync(id);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }
    }
}
