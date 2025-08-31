using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EscortGroupRepository : IEscortGroupRepository
    {
        private readonly AppDbContext _context;

        public EscortGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateGroupAsync(EscortJourneyGroup group)
        {
            _context.EscortJourneyGroups.Add(group);
            await _context.SaveChangesAsync();
            return group.Id;
        }

        public async Task DeleteGroupByGroupCodeAsync(string groupCode)
        {
            var group = await _context.EscortJourneyGroups      
        .       FirstOrDefaultAsync(g => g.GroupCode.Equals(groupCode));

            if (group == null)
                throw new Exception("Group not found");

            _context.EscortJourneyGroups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetGroupIdByGroupCodeAsync(string groupCode)
        {
            return await _context.EscortJourneyGroups
                .Where(g => g.GroupCode == groupCode)
                .Select(g => g.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<EscortGroupSettingsDto?> GetGroupSettingsByCodeAsync(string groupCode)
        {
            return await _context.EscortJourneyGroups
                .Where(g => g.GroupCode == groupCode)
                .Select(g => new EscortGroupSettingsDto
                {
                    Id = g.Id,
                    MaxMemberNumber = g.MaxMemberNumber,
                    AutoApprove = g.AutoApprove,
                    ReceiveRequest = g.ReceiveRequest,
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateGroupSettingsByCodeAsync(UpdateEscortGroupSettingsDTO groupSettings)
        {
            var group = await _context.EscortJourneyGroups
                .FirstOrDefaultAsync(g => g.GroupCode == groupSettings.groupCode);

            if (group == null)
                throw new Exception("Nhóm không tồn tại.");

            group.AutoApprove = groupSettings.AutoApprove;
            group.ReceiveRequest = groupSettings.ReceiveRequest;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsGroupCodeExistsAsync(string groupCode)
        {
            return await _context.EscortJourneyGroups.AnyAsync(g => g.GroupCode == groupCode);
        }

        public async Task<int> GetGroupCountByAccountIdAsync(Guid accountId)
        {
            return await _context.EscortJourneyGroupMembers
                .CountAsync(m => m.AccountId == accountId);
        }

        public async Task AddMemberAsync(EscortJourneyGroupMember member)
        {
            _context.EscortJourneyGroupMembers.Add(member);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAlreadyInGroupAsync(Guid accountId, int groupId)
        {
            return await _context.EscortJourneyGroupMembers
                .AnyAsync(m => m.AccountId == accountId && m.GroupId == groupId);
        }

        public async Task<EscortJourneyGroupMember?> GetMemberbyUserIdAndGroupIdAsync(Guid accountId, int groupId)
        {
            return await _context.EscortJourneyGroupMembers
                .FirstOrDefaultAsync(m => m.AccountId == accountId && m.GroupId == groupId);
        }

        public async Task<int> GetMemberCountAsync(int groupId)
        {
            return await _context.EscortJourneyGroupMembers
                .CountAsync(m => m.GroupId == groupId);
        }

        public async Task<List<EscortJourneyGroup>> GetGroupsByAccountIdAsync(Guid accountId)
        {
            return await _context.EscortJourneyGroups
                .Where(g => g.Members.Any(m => m.AccountId == accountId))
                .Include(g => g.Members)
                .ToListAsync();
        }

        public async Task<GroupWaitingRoomDto?> GetGroupWithLeaderAndMembersAsync(int groupId, Guid accountId)
        {
            return await _context.EscortJourneyGroups
                .Select(g => new GroupWaitingRoomDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    GroupCode = g.GroupCode,
                    AutoApprove = g.AutoApprove,
                    ReceiveRequest = g.ReceiveRequest,
                    MaxMemberNumber = g.MaxMemberNumber,
                    IsLeader = g.LeaderId == accountId,
                    CurrentMemberCount = g.Members.Count,
                    LeaderName = g.Leader.FullName,
                    CreatedAt = g.CreatedAt,
                    Members = g.Members.Select(m => new EscortGroupMemberDto
                    {
                        Id = m.Id,
                        FullName = m.Account.FullName,
                        Email = m.Account.Email,
                        AvatarUrl = m.Account.ImageUrl ?? "unavailable avatar",
                        Role = m.Role,                      
                    })
                })
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task RemoveGroupMemberByIdAsync(int memberId)
        {
            var member = await _context.EscortJourneyGroupMembers
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member != null)
            {
                _context.EscortJourneyGroupMembers.Remove(member);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Guid> GetLeaderUserIdAsync(int groupId)
        {
            return await _context.EscortJourneyGroups
                .Where(g => g.Id == groupId)
                .Select(g => g.LeaderId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetGroupIdByMemberIdAsync(int memberId)
        {
            return await _context.EscortJourneyGroupMembers
                . Where(g => g.Id == memberId)
                .Select (g => g.GroupId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateGroupMemberStatusAsync(EscortJourneyGroupMember groupMember, string status)
        {           
            groupMember.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetInvitersForWatcherAsync(int groupId, int watcherMemberId)
        {
            var inviterMemberIds = await _context.EscortJourneyWatchers
                .Where(w => w.WatcherId == watcherMemberId) 
                .Where(w => w.EscortJourney.CreatedInGroupId == groupId) 
                .Where(w => w.EscortJourney.Status.Equals("Active")) 
                .Select(w => w.EscortJourney.MemberId)
                .Distinct()
                .ToListAsync();

            return inviterMemberIds;
        }

    }

}
