using BusinessObject.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class BlogResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public BlogType Type { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Pinned { get; set; }
        public string DistrictName { get; set; }
        public List<string> MediaUrls { get; set; }
        public int TotalLike { get; set; }
        public int TotalComment { get; set; }
        public bool IsLike { get; set; } //Is the user like this blog
    }

}
