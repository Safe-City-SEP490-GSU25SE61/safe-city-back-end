using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class AchievementResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinPoint { get; set; }
        public string Benefit { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? ImageUrl { get; set; }
    }
}
