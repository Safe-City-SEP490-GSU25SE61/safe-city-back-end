using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BusinessObject.DTOs.Enums;

namespace BusinessObject.Models
{
    [Table("blog")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("author_id")]
        public Guid AuthorId { get; set; }

        [Column("commune_id")]
        public int CommuneId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("type")]
        public BlogType Type { get; set; } // 'Alert', 'Tip', 'Event', 'News'

        [Column("pinned")]
        public bool Pinned { get; set; } = false;

        [Column("is_approved")]
        public bool IsApproved { get; set; } = false;

        [Column("is_visible")]
        public bool IsVisible { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Account Author { get; set; }
        public Commune Commune { get; set; }
        public BlogModeration Moderation { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<BlogLike> Likes { get; set; } = new List<BlogLike>();
        public ICollection<BlogMedia> Media { get; set; } = new List<BlogMedia>();
    }
}
