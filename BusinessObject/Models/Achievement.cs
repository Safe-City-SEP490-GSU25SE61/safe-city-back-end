using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("achievement")]
    public class Achievement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }
        [Column("image_url")]
        public string? ImageUrl { get; set; }
        [Column("min_point")]
        public int MinPoint { get; set; }

        [Column("benefit")]
        public string Benefit { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("point_multiplier")]
        public double? PointMultiplier { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
}
