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

        [Column("member_id")]
        public int MemberId { get; set; }

        [Column("created_in_group_id")]
        public int? CreatedInGroupId { get; set; }

        [Column("vehicle")]
        public string Vehicle { get; set; }

        [Column("start_point")]
        public string StartPoint { get; set; }

        [Column("start_latitude")]
        public double? StartLatitude { get; set; }

        [Column("start_longitude")]
        public double? StartLongitude { get; set; }

        [Column("end_point")]
        public string EndPoint { get; set; }

        [Column("end_latitude")]
        public double? EndLatitude { get; set; }

        [Column("end_longitude")]
        public double? EndLongitude { get; set; }

        [Column("distance_in_meters")]
        public int DistanceInMeters { get; set; }

        [Column("duration_in_seconds")]
        public int DurationInSeconds { get; set; }

        [Column("start_time")]
        public DateTime? StartTime { get; set; }

        [Column("expected_end_time")]
        public DateTime? ExpectedEndTime { get; set; }

        [Column("arrival_time")]
        public DateTime? ArrivalTime { get; set; }

        [Column("route_json")]
        public string RouteJson { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Active";// "Active", "Completed", "Timeout"

        [Column("deviation_alert_sent")]
        public bool DeviationAlertSent { get; set; } = false;

        public Account User { get; set; }
        public EscortJourneyGroupMember Member { get; set; }
        public EscortJourneyGroup CreatedInGroup { get; set; }
        public ICollection<EscortJourneyWatcher> Watchers { get; set; }
        public ICollection<SosAlert> SosAlerts { get; set; }
        public ICollection<LocationHistory> LocationHistories { get; set; }

    }

}
