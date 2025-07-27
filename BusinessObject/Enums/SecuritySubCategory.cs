using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum SecuritySubCategory
    {
        [Display(Name = "Cướp giật")]
        Robbery,

        [Display(Name = "Trộm cắp")]
        Theft,

        [Display(Name = "Bạo lực")]
        Violence,

        [Display(Name = "Lừa đảo")]
        Fraud,

        [Display(Name = "Tệ nạn xã hội")]
        SocialEvil,

        [Display(Name = "Hoạt động đáng nghi")]
        SuspiciousActivity,

        [Display(Name = "Mất an ninh trật tự")]
        PublicDisturbance
    }

}
