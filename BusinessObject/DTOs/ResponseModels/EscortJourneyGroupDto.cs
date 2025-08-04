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
        public GroupMemberLimitTier MaxMemberNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EscortGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxMemberNumber { get; set; }
        public int MemberCount { get; set; }
    }

}
