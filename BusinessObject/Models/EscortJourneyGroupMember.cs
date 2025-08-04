using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("escort_journey_group_member")]
    public class EscortJourneyGroupMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("account_id")]
        public Guid AccountId { get; set; }

        [Column("role")]
        public GroupMemberRole Role { get; set; } = GroupMemberRole.Member;

        [Column("is_online")]
        public bool IsOnline { get; set; } = false;

        [Column("joined_at")]
        public DateTime? JoinedAt { get; set; }
        public EscortJourneyGroup Group { get; set; }
        public Account Account { get; set; }
    }
}
