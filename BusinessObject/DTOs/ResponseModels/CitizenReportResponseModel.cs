using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class CitizenReportResponseModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime OccurredAt { get; set; }
        public string Status { get; set; }
        public string? StatusMessage { get; set; }
        public bool IsAnonymous { get; set; }
        public List<string> ImageUrls { get; set; }
        public string? VideoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
