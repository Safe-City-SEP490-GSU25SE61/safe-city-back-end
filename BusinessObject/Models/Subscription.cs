using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("subscription")]
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("package_id")]
        public int PackageId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        public Account Account { get; set; }
        public Package Package { get; set; }
    }
}
