using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class PackageDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string Color { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool CanPostBlog { get; set; }
        public bool CanViewIncidentDetail { get; set; }
        public int MonthlyVirtualEscortLimit { get; set; }
        public bool CanReusePreviousEscortPaths { get; set; }
        public bool IsActive { get; set; }
    }
}
