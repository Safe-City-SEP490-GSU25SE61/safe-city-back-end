using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("assign_officer_history")]
    public class AssignOfficerHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("account_id")]
        public Guid AccountId { get; set; }

        [Column("old_district_id")]
        public int? OldDistrictId { get; set; }

        [Column("new_district_id")]
        public int NewDistrictId { get; set; }

        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }
    }
}
