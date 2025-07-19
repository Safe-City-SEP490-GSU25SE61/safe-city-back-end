using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ReportResponseModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? VerifiedByName { get; set; }
        public string? StatusMessage { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public List<string> Notes { get; set; }
        public List<string> ImageUrls { get; set; }
        public string? VideoUrl { get; set; }
        public List<RelatedReportResponseModel>? RelatedReports { get; set; }


    }
}
