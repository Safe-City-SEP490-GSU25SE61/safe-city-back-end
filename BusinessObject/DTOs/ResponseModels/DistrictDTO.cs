using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class DistrictDTO
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessage = "Tên không được dài hơn 100 ký tự.")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tổng số sự cố báo cáo phải là một số dương.")]
        public int TotalReportedIncidents { get; set; }

        [Range(0, 10, ErrorMessage = "Mức độ nguy hiểm phải nằm trong khoảng từ 0 đến 10.")]
        public int DangerLevel { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được dài hơn 500 ký tự.")]
        public string Note { get; set; }

        [StringLength(1000, ErrorMessage = "Dữ liệu Polygon không được dài hơn 1000 ký tự.")]
        [RegularExpression(@"^\(\([0-9]+\.[0-9]+ [0-9]+\.[0-9]+(, [0-9]+\.[0-9]+ [0-9]+\.[0-9]+)*\)\)$", ErrorMessage = "Dữ liệu Polygon phải theo định dạng hợp lệ.")]
        public string PolygonData { get; set; }


        public DateTime CreateAt { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsActive { get; set; }

        public List<WardDTO> Wards { get; set; }
    }
}


