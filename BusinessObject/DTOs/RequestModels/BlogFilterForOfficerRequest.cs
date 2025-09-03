using BusinessObject.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class BlogFilterForOfficerRequest
    {
        public string? Keyword { get; set; }
        public BlogType? Type { get; set; }
        public string SortOption { get; set; } = string.Empty;
    }

}
