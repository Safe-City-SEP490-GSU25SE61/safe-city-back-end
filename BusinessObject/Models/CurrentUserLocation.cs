using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("current_user_location")]
    public class CurrentUserLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("escort_journey_id")]
        public int EscortJourneyId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitude")]
        public decimal Longitude { get; set; }

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Column("speed")]
        public decimal? Speed { get; set; }

        [Column("heading")]
        public decimal? Heading { get; set; }

        [Column("battery_level")]
        public int? BatteryLevel { get; set; }

        public EscortJourney EscortJourney { get; set; }
        public Account User { get; set; }
    }

}
