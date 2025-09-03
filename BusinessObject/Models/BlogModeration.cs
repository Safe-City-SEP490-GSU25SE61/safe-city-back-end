using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("blog_moderation")]
    public class BlogModeration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("is_approved")]
        public bool IsApproved { get; set; }

        [Column("politeness")]
        public bool Politeness { get; set; }

        [Column("no_anti_state")]
        public bool NoAntiState { get; set; }

        [Column("positive_meaning")]
        public bool PositiveMeaning { get; set; }

        [Column("type_requirement")]
        public bool TypeRequirement { get; set; }

        [Column("reasoning")]
        public string Reasoning { get; set; }

        [Column("violations")]
        public string? ViolationsJson { get; set; } // Lưu mảng JSON ["câu 1", "câu 2",...]

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Blog Blog { get; set; }
    }

}
