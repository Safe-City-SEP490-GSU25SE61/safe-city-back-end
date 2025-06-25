using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdatePackageDTO
    {
        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        [StringLength(500, ErrorMessage = "Mô tả không được dài hơn 500 ký tự.")]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Giá phải là số dương.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thời gian gói dịch vụ phải là số dương.")]
        public int DurationDays { get; set; }

        [StringLength(50, ErrorMessage = "Màu không được dài hơn 50 ký tự.")]
        public string Color { get; set; }

    }
}
