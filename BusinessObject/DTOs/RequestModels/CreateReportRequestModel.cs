using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateReportRequestModel
    {
     
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Type { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public string Address { get; set; }

        public bool IsAnonymous { get; set; } = false;
        public List<IFormFile>? Images { get; set; }
    }
}
