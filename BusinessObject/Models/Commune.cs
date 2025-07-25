using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("commune")]
    public class Commune
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("total_reported_incidents")]
        public int TotalReportedIncidents { get; set; }

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
        public ICollection<IncidentReport> IncidentReports { get; set; }
        public ICollection<Blog> Blogs { get; set; }
    }
}
