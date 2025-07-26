using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Enums
{
    public enum EnvironmentSubCategory
    {
        [Display(Name = "Ô nhiễm không khí")]
        AirPollution,

        [Display(Name = "Ô nhiễm nước")]
        WaterPollution,

        [Display(Name = "Tiếng ồn")]
        NoisePollution,

        [Display(Name = "Rác thải bừa bãi")]
        GarbageDump,

        [Display(Name = "Xả thải trái phép")]
        IllegalDumping,

        [Display(Name = "Cháy rừng")]
        ForestFire,

        [Display(Name = "Động vật chết")]
        DeadAnimal
    }

}
