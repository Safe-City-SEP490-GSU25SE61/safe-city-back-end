using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum TrafficSubCategory
    {
        [Display(Name = "Tai nạn giao thông")]
        TrafficAccident,

        [Display(Name = "Ùn tắc giao thông")]
        TrafficCongestion,

        [Display(Name = "Đèn tín hiệu hỏng")]
        BrokenTrafficLight,

        [Display(Name = "Biển báo hư hỏng")]
        BrokenSignage,

        [Display(Name = "Đường bị hư hỏng")]
        RoadDamage,

        [Display(Name = "Vật cản trên đường")]
        RoadObstacle,

        [Display(Name = "Xe dừng đỗ sai quy định")]
        IllegalParking
    }

}
