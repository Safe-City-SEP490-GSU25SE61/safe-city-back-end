using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("district")]
    public class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("total_reported_incidents")]
        public int TotalReportedIncidents { get; set; }

        [Column("danger_level")]
        public int DangerLevel { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("polygon_data")]
        public string PolygonData { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        public ICollection<Account> Accounts { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
}
