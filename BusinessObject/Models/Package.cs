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

        [Display(Name = "Mô tả")]
        [Column("description")]
        public string Description { get; set; }

        [Display(Name = "Giá")]
        [Column("price")]
        public decimal Price { get; set; }

        [Display(Name = "Số ngày sử dụng")]
        [Column("duration_days")]
        public int DurationDays { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("can_post_blog")]
        public bool CanPostBlog { get; set; } 

        [Column("can_view_incident_detail")]
        public bool CanViewIncidentDetail { get; set; }

        [Column("monthly_virtual_escort_limit")]
        public int MonthlyVirtualEscortLimit { get; set; }

        [Column("can_reuse_previous_escort_paths")]
        public bool CanReusePreviousEscortPaths { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
