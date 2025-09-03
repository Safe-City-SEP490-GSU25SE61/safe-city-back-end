using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("blog_media")]
    public class BlogMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("type")]
        public string Type { get; set; } // 'image' or 'video'

        [Column("media_slot")]
        public int MediaSlot { get; set; } // from 1 to 4

        [Column("file_data")]
        public string FileUrl { get; set; }

        [Column("thumbnail_data")]
        public string? ThumbnailUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        public Blog Blog { get; set; }
    }
}
