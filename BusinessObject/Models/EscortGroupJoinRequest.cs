using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("escort_group_join_request")]
    public class EscortGroupJoinRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("group_id")]
        public int GroupId { get; set; }

        [Column("account_id")]
        public Guid AccountId { get; set; }

        [Column("requested_at")]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        [Column("is_approved")]
        public bool? IsApproved { get; set; } = null; // null = pending, true = approved, false = rejected

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

        public EscortJourneyGroup Group { get; set; }
        public Account Account { get; set; }
    }
}
