using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPointHistoryService
    {
        Task<long> LogAsync(Guid userId, Guid? actorId, string sourceType, string? sourceId, string action, int pointsDelta, int reputationDelta, string? note = null);
        Task<PointHistoryResponseDto> GetHistoryAsync(Guid userId, string? range, string? sourceType, bool desc = true);
    }
}
