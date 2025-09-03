using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class PointHistoryItemDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }       
        public int PointsDelta { get; set; }
        public int ReputationDelta { get; set; }
        public string SourceType { get; set; } = default!;   
        public string Action { get; set; } = default!;       
        public string? Note { get; set; }
        public string? ActorName { get; set; }
        public object? Source { get; set; }          
    }

    public class PointHistoryResponseDto
    {
        public Guid UserId { get; set; }
        public int CurrentTotalPoint { get; set; }
        public int CurrentReputationPoint { get; set; }
        public IEnumerable<PointHistoryItemDto> Items { get; set; } = new List<PointHistoryItemDto>();

    }
}

