using BusinessObject.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateReportRequestModel
    {
     
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IncidentType Type { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public decimal? Lat { get; set; }
        [Required]
        public decimal? Lng { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public DateTime OccurredAt { get; set; }
        public bool IsAnonymous { get; set; } = false;
        public List<IFormFile>? Images { get; set; }
        public IFormFile? Video { get; set; }

    }
}
