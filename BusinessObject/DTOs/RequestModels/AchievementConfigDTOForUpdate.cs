using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class AchievementConfigDTOForUpdate
    {
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "MinPoint must be a positive number.")]
        public int MinPoint { get; set; }

        [Required(ErrorMessage = "Benefit is required.")]
        [StringLength(500, ErrorMessage = "Benefit cannot be longer than 500 characters")]
        public string Benefit { get; set; }
    }
}
