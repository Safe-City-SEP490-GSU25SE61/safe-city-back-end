using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("escort_journey")]
    public class EscortJourney
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("created_in_group_id")]
        public int? CreatedInGroupId { get; set; }

        [Column("start_point")]
        public string StartPoint { get; set; }

        [Column("start_latitude")]
        public decimal? StartLatitude { get; set; }

        [Column("start_longitude")]
        public decimal? StartLongitude { get; set; }

        [Column("end_point")]
        public string EndPoint { get; set; }

        [Column("end_latitude")]
        public decimal? EndLatitude { get; set; }

        [Column("end_longitude")]
        public decimal? EndLongitude { get; set; }

        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        [Column("expected_time")]
        public DateTime? ExpectedTime { get; set; }

        [Column("expected_end_time")]
        public DateTime? ExpectedEndTime { get; set; }

        [Column("arrival_time")]
        public DateTime? ArrivalTime { get; set; }

        [Column("status")]
        public string Status { get; set; } // "Active", "Completed", "Timeout"

        [Column("deviation_alert_sent")]
        public bool DeviationAlertSent { get; set; } = false;

        public Account User { get; set; }
        public EscortJourneyGroup CreatedInGroup { get; set; }
        public ICollection<EscortJourneyWatcher> Watchers { get; set; }
        public ICollection<SosAlert> SosAlerts { get; set; }
        public ICollection<LocationHistory> LocationHistories { get; set; }
        public ICollection<CurrentUserLocation> CurrentUserLocations { get; set; }

    }

}
