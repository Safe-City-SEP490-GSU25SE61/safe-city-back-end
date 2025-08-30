using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public EscortJourney EscortJourney { get; set; }
        public EscortJourneyGroupMember Watcher { get; set; }
    }

}
