using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("package")]
    public class Package
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("duration_days")]
        public int DurationDays { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }


        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
