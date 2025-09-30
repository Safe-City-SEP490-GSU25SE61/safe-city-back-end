using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BusinessObject.Models
{
    [Table("escort_journey_watcher")]
    public class EscortJourneyWatcher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("escort_journey_id")]
        public int EscortJourneyId { get; set; }

        [Column("watcher_id")]
        public int WatcherId { get; set; }

        [Column("added_at")]
        public DateTime AddedAt { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("call_session_name")]
        public string? CallSessionName { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        [Column("join_time")]
        public DateTime? JoinTime { get; set; }

        [Column("leave_time")]
        public DateTime? LeaveTime { get; set; }

        [Column("call_status")]
        public string? CallStatus { get; set; }

        [Column("agora_uid")]
        public string? AgoraUid { get; set; }

        [Column("token")]
        public string? Token { get; set; }

        [Column("issued_at")]
        public DateTime? IssuedAt { get; set; }

        [Column("expire_at")]
        public DateTime? ExpireAt { get; set; }

        [Column("last_refreshed_at")]
        public DateTime? LastRefreshedAt { get; set; }

        [JsonIgnore] 
        public EscortJourney EscortJourney { get; set; }
        public EscortJourneyGroupMember Watcher { get; set; }
    }

}
