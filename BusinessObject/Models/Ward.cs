using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("ward")]
    public class Ward
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("district_id")]
        public int DistrictId { get; set; }

        [Column("total_reported_incidents")]
        public int TotalReportedIncidents { get; set; }

        [Column("danger_level")]
        public int DangerLevel { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("polygon_data")]
        public string PolygonData { get; set; }


        public District District { get; set; }
    }
}
