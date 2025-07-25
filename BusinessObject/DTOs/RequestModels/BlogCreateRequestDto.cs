using BusinessObject.DTOs.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class BlogCreateRequestDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(8000, ErrorMessage = "Content cannot be longer than 8000 characters.")]
        [MinLength(5, ErrorMessage = "Content must be at least 5 characters long.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public BlogType Type { get; set; }

        [Required(ErrorMessage = "Commune is required.")]
        public int CommuneId { get; set; }
        public List<IFormFile>? MediaFiles { get; set; }
    }
}
