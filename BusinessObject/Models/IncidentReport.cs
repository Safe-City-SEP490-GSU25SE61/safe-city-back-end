using BusinessObject.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("incident_report")]
    public class IncidentReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("district_id")]
        public int? DistrictId { get; set; }

        [Column("ward_id")]
        public int? WardId { get; set; }

        [Column("type")]
        public IncidentType Type { get; set; }  

        [Column("description")]
        public string Description { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("lat")]
        public decimal? Lat { get; set; }

        [Column("lng")]
        public decimal? Lng { get; set; }

        [Column("is_anonymous")]
        public bool IsAnonymous { get; set; }

        [Column("status")]
        public string Status { get; set; }  

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("verified_by")]
        public Guid? VerifiedBy { get; set; }
        [Column("status_message")]
        public string? StatusMessage { get; set; }
        [Column("image_urls")]
        public string? ImageUrls { get; set; }
        [Column("video_url")]
        public string? VideoUrl { get; set; }


        public Account User { get; set; }
        public Account? Verifier { get; set; }
        public District District { get; set; }
        public Ward Ward { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
