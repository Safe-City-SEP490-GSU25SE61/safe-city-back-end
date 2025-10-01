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
        Task<(string senderName, string token, string channelName, int alertId)> CreateAlertAsync(int escortJourneyId, Guid senderId, decimal lat, decimal lng, DateTime timestamp);
        Task EndSosCallAsync(int sosAlertId);
        //Task RefreshTokensForSosAsync(int sosAlertId, int expireInSeconds = 3600);
        Task<(string? channelName, string? token)> JoinWatcherAsync(int sosAlertId, Guid userId);
        Task LeaveWatcherAsync(int sosAlertId, Guid userId);
        Task<SosAlert?> GetLatestAlertBySenderIdAsync(Guid senderId);
    }
}
