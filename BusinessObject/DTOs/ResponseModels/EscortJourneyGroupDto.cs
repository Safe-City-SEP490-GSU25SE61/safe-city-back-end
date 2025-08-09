using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class EscortJourneyGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public int MaxMemberNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EscortGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxMemberNumber { get; set; }
        public int MemberCount { get; set; }
        public string Role {  get; set; }
    }

    public class GroupWaitingRoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public int MaxMemberNumber { get; set; }
        public int CurrentMemberCount { get; set; }
        public string LeaderName { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<EscortGroupMemberDto> Members { get; set; }
    }

    public class EscortGroupMemberDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public GroupMemberRole Role { get; set; }
        //public bool IsOnline { get; set; }
    }


}
