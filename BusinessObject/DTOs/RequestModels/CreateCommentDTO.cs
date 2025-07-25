using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateCommentDTO
    {
        [Required(ErrorMessage = "Blog id is required.")]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(100, ErrorMessage = "Content cannot be longer than 100 characters.")]
        public string Content { get; set; } = null!;
    }
}
