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
        [Display(Name = "Xả rác")]
        Littering,

        [Display(Name = "Kẹt xe")]
        TrafficJam,

        [Display(Name = "Tai nạn giao thông")]
        Accident,

        [Display(Name = "Đánh nhau")]
        Fighting,

        [Display(Name = "Trộm cắp")]
        Theft,

        [Display(Name = "Gây rối trật tự")]
        PublicDisorder,

        [Display(Name = "Phá hoại công trình")]
        Vandalism,

        [Display(Name = "Khác")]
        Other
    }

}
