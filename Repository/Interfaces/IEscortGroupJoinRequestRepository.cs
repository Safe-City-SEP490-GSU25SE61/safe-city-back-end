using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IEscortGroupJoinRequestRepository
    {   
        Task<bool> ExistsAsync(Guid accountId, int groupId);
        Task<int> AddAsync(EscortGroupJoinRequest request);
        Task<List<PendingRequestDto>> GetPendingRequestsByGroupIdAsync(int groupId);
        Task<EscortGroupJoinRequest> GetByIdAsync(int requestId);
        Task ReviewAsync(EscortGroupJoinRequest request);
    }

}
