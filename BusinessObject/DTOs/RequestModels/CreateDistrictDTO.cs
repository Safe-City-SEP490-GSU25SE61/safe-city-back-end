using System.ComponentModel.DataAnnotations;

namespace BusinessObject.DTOs
{
    public class CreateDistrictDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "TotalReportedIncidents must be a positive number")]
        public int TotalReportedIncidents { get; set; }

        [Range(0, 10, ErrorMessage = "DangerLevel must be between 0 and 10")]
        public int DangerLevel { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot be longer than 500 characters")]
        public string Note { get; set; }

        [StringLength(1000, ErrorMessage = "PolygonData cannot be longer than 1000 characters")]
        [RegularExpression(@"^\(\([0-9]+\.[0-9]+ [0-9]+\.[0-9]+(, [0-9]+\.[0-9]+ [0-9]+\.[0-9]+)*\)\)$", ErrorMessage = "PolygonData must be a valid polygon format.")]
        public string PolygonData { get; set; }
    }
}
