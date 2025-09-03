using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum IncidentType
    {
        [Display(Name = "Giao thông")]
        Traffic,

        [Display(Name = "An ninh")]
        Security,

        [Display(Name = "Cơ sở hạ tầng")]
        Infrastructure,

        [Display(Name = "Môi trường")]
        Environment,

        [Display(Name = "Khác")]
        Other
    }

}
