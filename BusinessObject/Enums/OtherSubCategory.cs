using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum OtherSubCategory
    {
        [Display(Name = "Sự cố y tế")]
        MedicalIssue,

        [Display(Name = "Thiên tai")]
        NaturalDisaster,

        [Display(Name = "Cháy nổ")]
        Explosion,

        [Display(Name = "Mất điện, nước")]
        PowerWaterOutage,

        [Display(Name = "Khác")]
        Misc
    }

}
