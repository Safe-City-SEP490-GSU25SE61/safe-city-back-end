using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("point_history")]
    public class PointHistory
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("actor_id")]
        public Guid? ActorId { get; set; }

        [Column("source_type")]
        public string SourceType { get; set; } = default!;   

        [Column("source_id")]
        public string? SourceId { get; set; }               

        [Column("action")]
        public string Action { get; set; } = default!;      

        [Column("points_delta")]
        public int PointsDelta { get; set; }                

        [Column("reputation_delta")]
        public int ReputationDelta { get; set; }            

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Account User { get; set; }
        public Account? Actor { get; set; }
    }
}
