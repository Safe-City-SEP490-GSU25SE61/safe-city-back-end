using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ILocationHistoryRepository
    {
        Task AddAsync(LocationHistory locationHistory);
        Task<List<LocationHistory>> GetByJourneyIdAsync(int escortJourneyId);
    }

}
