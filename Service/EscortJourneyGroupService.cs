using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using BusinessObject.Models;
using Repository;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EscortJourneyGroupService : IEscortJourneyGroupService
    {
        private readonly IEscortGroupRepository _groupRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IEscortGroupJoinRequestRepository _groupJoinRequestRepository;

        public EscortJourneyGroupService(IEscortGroupRepository repository, IAccountRepository accountRepository,
            ISubscriptionRepository subscriptionRepository, IEscortGroupJoinRequestRepository groupJoinRequestRepository)
        {
            _groupRepository = repository;
            _accountRepository = accountRepository;
            _subscriptionRepository = subscriptionRepository;
            _groupJoinRequestRepository = groupJoinRequestRepository;
        }

        public async Task CreateGroupAsync(Guid accountId, CreateEscortJourneyGroupRequest request)
        {
            int currentGroups = await _groupRepository.GetGroupCountByAccountIdAsync(accountId);
            int maxAllowed = 5;

            if (currentGroups >= maxAllowed)
                throw new InvalidOperationException("Bạn chỉ có thể tham gia tối đa 5 nhóm.");

            string groupCode;
            do
            {
                groupCode = GenerateRandomCode();
            } while (await _groupRepository.IsGroupCodeExistsAsync(groupCode));

            var group = new EscortJourneyGroup
            {
                Name = request.Name,
                GroupCode = groupCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LeaderId = accountId
            };

            var groupId = await _groupRepository.CreateGroupAsync(group);

            await _groupRepository.AddMemberAsync(new EscortJourneyGroupMember
            {
                AccountId = accountId,
                GroupId = groupId,
                Role = GroupMemberRole.Leader,
                IsOnline = true,
                JoinedAt = DateTime.UtcNow
            });
        }

        public async Task JoinGroupAsync(Guid accountId, string code)
        {
            var groupId = await _groupRepository.GetGroupIdByCodeAsync(code)
                         ?? throw new KeyNotFoundException("Không tìm thấy nhóm.");

            if (await _groupRepository.IsAlreadyInGroupAsync(accountId, groupId))
                throw new InvalidOperationException("Bạn đã tham gia nhóm này.");
            if (await _groupJoinRequestRepository.ExistsAsync(accountId, groupId))
                throw new InvalidOperationException("Bạn đã gửi yêu cầu tham gia nhóm này.");

            int currentGroups = await _groupRepository.GetGroupCountByAccountIdAsync(accountId);
            int maxAllowed = 5;

            if (currentGroups >= maxAllowed)
                throw new InvalidOperationException("Bạn chỉ có thể tham gia tối đa 5 nhóm.");

            var request = new EscortGroupJoinRequest
            {
                AccountId = accountId,
                GroupId = groupId,
                IsApproved = null,
            };

            await _groupJoinRequestRepository.AddAsync(request);
        }

        public async Task ReviewJoinRequestAsync(int requestId, bool approve)
        {
            var request = await _groupJoinRequestRepository.GetByIdAsync(requestId);

            if (request.IsApproved != null)
                throw new InvalidOperationException("Yêu cầu này đã được xử lý.");

            request.IsApproved = approve;
            request.ReviewedAt = DateTime.UtcNow;

            if (approve)
            {
                int currentMemberCount = await _groupRepository.GetMemberCountAsync(request.GroupId);
                int maxAllowed = (int)request.Group.MaxMemberNumber;

                if (currentMemberCount >= maxAllowed)
                    throw new InvalidOperationException("Nhóm đã đầy.");

                await _groupRepository.AddMemberAsync(new EscortJourneyGroupMember
                {
                    GroupId = request.GroupId,
                    AccountId = request.AccountId,
                    JoinedAt = DateTime.UtcNow
                });
            }

            await _groupJoinRequestRepository.ReviewAsync(request);
        }

        public async Task<List<PendingRequestDto>> GetPendingRequestsByGroupIdAsync(int groupId)
        {
            var requests = await _groupJoinRequestRepository.GetPendingRequestsByGroupIdAsync(groupId);

            return requests;
        }

        public async Task<List<EscortGroupDto>> GetGroupsByAccountIdAsync(Guid accountId)
        {
            var groups = await _groupRepository.GetGroupsByAccountIdAsync(accountId);

            return groups.Select(g => new EscortGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                MaxMemberNumber = (int)g.MaxMemberNumber,
                MemberCount = g.Members.Count,
                Role = (g.LeaderId == accountId ? GroupMemberRole.Leader : GroupMemberRole.Member).ToString(),
            }).ToList();
        }

        public async Task<GroupWaitingRoomDto?> GetGroupWaitingRoomAsync(int groupId)
        {
            var group = await _groupRepository.GetGroupWithLeaderAndMembersAsync(groupId);
            if (group == null) return null;

            return group;
        }


        public async Task DeleteGroupByIdAsync(string groupCode)
        {
            await _groupRepository.DeleteGroupByIdAsync(groupCode);
        }

        private string GenerateRandomCode()
        {
            return Guid.NewGuid().ToString("N")[..6].ToUpper();
        }
    }

}
