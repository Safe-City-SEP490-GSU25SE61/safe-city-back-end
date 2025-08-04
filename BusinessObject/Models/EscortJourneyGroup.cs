using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("escort_journey_group")]
    public class EscortJourneyGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("leader_id")]
        public Guid LeaderId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("group_code")]
        public string GroupCode { get; set; }

        [Column("max_member_number")]
        public GroupMemberLimitTier MaxMemberNumber { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Account Leader { get; set; }
        public ICollection<EscortJourneyGroupMember> Members { get; set; }
        public ICollection<EscortGroupJoinRequest> JoinRequests { get; set; }
    }
}
