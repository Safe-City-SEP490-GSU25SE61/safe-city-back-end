using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class CreateJourneyDTO
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public string RawJson { get; set; }

        [Required]
        public string Vehicle {  get; set; }
        public List<int> WatcherIds { get; set; } = new();
    }
}
