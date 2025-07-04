using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdateReportStatusRequestModel
    {
        [Required]
        [RegularExpression("^(verified|rejected)$", ErrorMessage = "Status must be verified or rejected.")]
        public string Status { get; set; }

    }
}
