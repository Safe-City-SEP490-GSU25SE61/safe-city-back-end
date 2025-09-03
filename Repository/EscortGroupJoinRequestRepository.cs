using BusinessObject.DTOs.RequestModels;
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
    public class EscortGroupJoinRequestRepository : IEscortGroupJoinRequestRepository
    {
        private readonly AppDbContext _context;

        public EscortGroupJoinRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid accountId, int groupId)
        {
            return await _context.EscortGroupJoinRequests
                .AnyAsync(r => r.AccountId == accountId && r.GroupId == groupId && r.IsApproved == null);
        }

        public async Task<int> AddAsync(EscortGroupJoinRequest request)
        {
            _context.EscortGroupJoinRequests.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }
        public async Task<EscortGroupJoinRequest?> GetByIdAsync(int requestId)
        {
            return await _context.EscortGroupJoinRequests
                .Include(r => r.Group)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.IsApproved == null);
        }
        public async Task<List<PendingRequestDto>> GetPendingRequestsByGroupIdAsync(int groupId)
        {
            return await _context.EscortGroupJoinRequests
                .Where(r => r.GroupId == groupId && r.IsApproved == null)
                .Select(r => new PendingRequestDto
                {
                    Id = r.Id,
                    AccountId = r.AccountId,
                    AccountName = r.Account.FullName,
                    RequestedAt = r.RequestedAt
                })
                .ToListAsync();
        }

        public async Task ReviewAsync(EscortGroupJoinRequest request)
        {
            await _context.SaveChangesAsync();
        }
    }

}
