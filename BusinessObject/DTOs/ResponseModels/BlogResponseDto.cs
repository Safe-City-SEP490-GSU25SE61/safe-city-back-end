using BusinessObject.DTOs.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string? AvaterUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Pinned { get; set; }
        public string CommuneName { get; set; }
        public string ProvinceName { get; set; }
        public List<string> MediaUrls { get; set; }
        public int TotalLike { get; set; }
        public int TotalComment { get; set; }
        public bool IsLike { get; set; }
    }

    public class BlogResponseForOfficerDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public BlogType Type { get; set; }
        public string AuthorName { get; set; }
        public bool Pinned { get; set; } 
        public bool IsApproved { get; set; } 
        public bool IsVisible { get; set; } 
        public DateTime CreatedAt { get; set; }
    }

    public class BlogModerationResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public BlogType Type { get; set; }
        public string AuthorName { get; set; }
        public int LikeNumber { get; set; }
        public int CommentNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> MediaUrls { get; set; }
        public BlogModeration BlogModeration { get; set; }
    }
}
