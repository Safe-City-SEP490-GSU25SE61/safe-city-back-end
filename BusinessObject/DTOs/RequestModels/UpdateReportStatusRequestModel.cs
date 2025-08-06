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
        [RegularExpression("^(verified|closed|malicious|solved)$",
        ErrorMessage = "Trạng thái không hợp lệ. Chỉ chấp nhận: verified, closed, malicious, solved.")]
        public string Status { get; set; }
        public string? Message { get; set; }
        public bool? IsVisibleOnMap { get; set; } 
    }
    public class CancelReportRequestModel
    {
        public string? Message { get; set; }
    }
}
