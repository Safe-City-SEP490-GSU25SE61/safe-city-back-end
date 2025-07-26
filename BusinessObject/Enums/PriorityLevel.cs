using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum PriorityLevel
    {
        [Display(Name = "Thấp")]
        Low,

        [Display(Name = "Trung bình")]
        Medium,

        [Display(Name = "Cao")]
        High,

        [Display(Name = "Khẩn cấp")]
        Critical
    }

}
