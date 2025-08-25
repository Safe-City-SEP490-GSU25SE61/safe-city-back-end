using System.ComponentModel.DataAnnotations;

namespace BusinessObject.DTOs
{
    public class CreateDistrictDTO
    {
        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên không được dài hơn 100 ký tự.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được dài hơn 500 ký tự.")]
        public string Note { get; set; }

        public string PolygonData { get; set; }
    }
}
