using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IEscortGroupRepository
    {
        Task<int> CreateGroupAsync(EscortJourneyGroup group);
        Task DeleteGroupByIdAsync(int groupId);
        Task<int?> GetGroupIdByCodeAsync(string groupCode);
        Task<bool> IsGroupCodeExistsAsync(string groupCode);
        Task<int> GetGroupCountByAccountIdAsync(Guid accountId);
        Task AddMemberAsync(EscortJourneyGroupMember member);
        Task<bool> IsAlreadyInGroupAsync(Guid accountId, int groupId);
        Task<List<EscortJourneyGroup>> GetGroupsByUserIdAsync(Guid userId);
        Task<int> GetMemberCountAsync(int groupId);
        Task<List<EscortJourneyGroup>> GetGroupsByAccountIdAsync(Guid accountId);

    }
}
