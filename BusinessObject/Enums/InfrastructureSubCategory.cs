using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum InfrastructureSubCategory
    {
        [Display(Name = "Ổ gà, đường hư")]
        Pothole,

        [Display(Name = "Cống thoát nước tắc")]
        BlockedDrain,

        [Display(Name = "Đèn đường hỏng")]
        BrokenStreetLight,

        [Display(Name = "Cây đổ, cành gãy")]
        FallenTree,

        [Display(Name = "Hư hỏng vỉa hè")]
        SidewalkDamage,

        [Display(Name = "Thiết bị công cộng hỏng")]
        BrokenPublicDevice,

        [Display(Name = "Công trình nguy hiểm")]
        DangerousConstruction
    }

}
