using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class AchievementConfigDTO
    {
        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên không được dài hơn 100 ký tự.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        [StringLength(500, ErrorMessage = "Mô tả không được dài hơn 500 ký tự.")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Điểm tối thiểu phải là một số dương.")]
        public int MinPoint { get; set; }

        [Required(ErrorMessage = "Lợi ích là bắt buộc.")]
        [StringLength(500, ErrorMessage = "Lợi ích không được dài hơn 500 ký tự.")]
        public string Benefit { get; set; }
    }
}
