using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class MapCommuneDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Polygon { get; set; }
    }

    public class MapReportDTO
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
    public class MapReportDetailDTO
    {
        public Guid Id { get; set; }
        public int? CommuneId { get; set; }
        public string Type { get; set; }
        public string? SubCategory { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public DateTime OccurredAt { get; set; }
        public string Status { get; set; }
    }
    public class MapReportResponse
    {
        public Dictionary<string, int>? ReportsByType { get; set; }
        public Dictionary<string, int>? ReportsByCommune { get; set; }
    }
    public class MapReportFilterQuery
    {
        [Required]
        public int CommuneId { get; set; }

        public IncidentType? Type { get; set; } 

        public string? Range { get; set; }
    }


}
