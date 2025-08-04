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

        public async Task DeleteGroupByIdAsync(int groupId)
        {
            var group = await _context.EscortJourneyGroups      
        .       FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                throw new Exception("Group not found");

            _context.EscortJourneyGroups.Remove(group);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetGroupIdByCodeAsync(string groupCode)
        {
            var group = await _context.EscortJourneyGroups
                .FirstOrDefaultAsync(g => g.GroupCode == groupCode);
            return group.Id;
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

        public async Task<int> GetMemberCountAsync(int groupId)
        {
            return await _context.EscortJourneyGroupMembers
                .CountAsync(m => m.GroupId == groupId);
        }

        public async Task<List<EscortJourneyGroup>> GetGroupsByUserIdAsync(Guid userId)
        {
            return await _context.EscortJourneyGroups
                .Where(g => g.Members.Any(m => m.AccountId == userId))
                .ToListAsync();
        }

        public async Task<List<EscortJourneyGroup>> GetGroupsByAccountIdAsync(Guid accountId)
        {
            return await _context.EscortJourneyGroups
                .Where(g => g.Members.Any(m => m.AccountId == accountId))
                .Include(g => g.Members)
                .ToListAsync();
        }

    }

}
