using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("sos_alert")]
    public class SosAlert
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("escort_journey_id")]
        public int EscortJourneyId { get; set; }

        [Column("sender_id")]
        public Guid SenderId { get; set; }

        [Column("lat")]
        public decimal Lat { get; set; }

        [Column("lng")]
        public decimal Lng { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        public EscortJourney EscortJourney { get; set; }
        public Account Sender { get; set; }
    }

}
