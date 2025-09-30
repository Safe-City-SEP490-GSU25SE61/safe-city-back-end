using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class SosCreateRequest
    {
        public int EscortJourneyId { get; set; }
        public Guid SenderId { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class SosStartResponse
    {
        public int SosAlertId { get; set; }
        public string ChannelName { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public List<WatcherTokenInfo> WatcherTokens { get; set; } = new();
    }

    public class WatcherTokenInfo
    {
        public int WatcherId { get; set; }            // EscortJourneyWatcher.WatcherId
        public int EscortWatcherRecordId { get; set; } // EscortJourneyWatcher.Id
        public string? Token { get; set; }
        public string? AgoraUid { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
