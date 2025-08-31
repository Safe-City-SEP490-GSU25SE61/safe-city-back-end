using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs.RequestModels;

namespace Repository.Interfaces
{
    public interface IEscortGroupRepository
    {
        Task<int> CreateGroupAsync(EscortJourneyGroup group);
        Task DeleteGroupByGroupCodeAsync(string groupCode);
        Task<int?> GetGroupIdByGroupCodeAsync(string groupCode);
        Task<EscortGroupSettingsDto?> GetGroupSettingsByCodeAsync(string groupCode);
        Task UpdateGroupSettingsByCodeAsync(UpdateEscortGroupSettingsDTO groupSettings);
        Task UpdateGroupMemberStatusAsync(EscortJourneyGroupMember groupMember, string status);
        Task<bool> IsGroupCodeExistsAsync(string groupCode);
        Task<int> GetGroupCountByAccountIdAsync(Guid accountId);
        Task AddMemberAsync(EscortJourneyGroupMember member);
        Task<bool> IsAlreadyInGroupAsync(Guid accountId, int groupId);
        Task<EscortJourneyGroupMember?> GetMemberbyUserIdAndGroupIdAsync(Guid accountId, int groupId);
        Task<int> GetMemberCountAsync(int groupId);
        Task<List<EscortJourneyGroup>> GetGroupsByAccountIdAsync(Guid accountId);
        Task<GroupWaitingRoomDto?> GetGroupWithLeaderAndMembersAsync(int groupId, Guid accountId);
        Task<Guid> GetLeaderUserIdAsync(int groupId);
        Task<int?> GetGroupIdByMemberIdAsync(int memberId);
        Task RemoveGroupMemberByIdAsync(int memberId);
        Task<List<int>> GetInvitersForWatcherAsync(int groupId, int watcherMemberId);

    }
}
