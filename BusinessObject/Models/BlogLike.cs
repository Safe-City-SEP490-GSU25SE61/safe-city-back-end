using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessObject.Models
{
    [Table("blog_like")]
    [Index(nameof(UserId), nameof(BlogId), IsUnique = true)]
    public class BlogLike
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("liked_at")]
        public DateTime LikedAt { get; set; }

        public Account User { get; set; }
        public Blog Blog { get; set; }
    }
}
