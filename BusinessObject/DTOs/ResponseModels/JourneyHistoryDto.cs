using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class JourneyHistoryDto
    {
        public List<EscortJourneyDto> EscortGroupDtos { get; set; } = new();
        public bool CanReusePreviousEscortPaths { get; set; }
    }

    public class EscortJourneyDto
    {
        public int Id { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Vehicle { get; set; }
        public string Status { get; set; }

        public List<WatcherDto> Watchers { get; set; } = new();
    }
    public class WatcherDto
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }
    }
}
