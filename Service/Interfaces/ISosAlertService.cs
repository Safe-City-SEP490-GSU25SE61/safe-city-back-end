using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISosAlertService
    {
        Task<string> CreateAlertAsync(int escortJourneyId, Guid senderId, decimal lat, decimal lng, DateTime timestamp);
    }
}
