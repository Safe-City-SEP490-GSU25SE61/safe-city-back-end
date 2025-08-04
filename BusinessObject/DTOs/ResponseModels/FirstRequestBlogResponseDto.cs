using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class FirstRequestBlogResponseDto
    {
        public IEnumerable<ProvinceDto> Provinces { get; set; }
        public IEnumerable<BlogResponseDto> Blogs { get; set; }
    }
}
