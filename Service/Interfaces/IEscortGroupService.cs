using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;

namespace Service.Interfaces
{
    public interface IEscortGroupService
    {
        Task CreateGroupAsync(Guid accountId, CreateEscortJourneyGroupRequest request);
        Task JoinGroupAsync(Guid accountId, string code);
        Task ReviewJoinRequestAsync(int requestId, bool approve);
        Task<List<PendingRequestDto>> GetPendingRequestsByGroupIdAsync(int groupId);
        Task<List<EscortGroupDto>> GetGroupsByAccountIdAsync(Guid accountId);
        Task DeleteGroupByIdAsync(string groupCode);
        Task KickMemberAsync(int memberId, Guid leaderId);
        Task<GroupWaitingRoomDto?> GetGroupWaitingRoomAsync(int groupId, Guid accountId);
        Task UpdateGroupSettingsAsync(UpdateEscortGroupSettingsDTO groupSettings, Guid leaderId);
    }
}
