using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IWatcherRepository
    {
        Task<List<EscortJourneyWatcher>> GetWatchersByJourneyIdAsync(int escortJourneyId);
        Task<List<EscortJourneyWatcher>> GetWatchersBySosAlertAsync(int sosAlertId, int? escortJourneyId = null);
        Task UpdateAsync(EscortJourneyWatcher watcher);
        Task<EscortJourneyWatcher?> GetByIdAsync(int id);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
