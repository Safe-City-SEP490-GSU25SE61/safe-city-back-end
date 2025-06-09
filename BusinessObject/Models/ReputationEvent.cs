using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("reputation_event")]
    public class ReputationEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }  

        [Column("action")]
        public string Action { get; set; }  

        [Column("points")]
        public int Points { get; set; }  

        [Column("source_type")]
        public string SourceType { get; set; } 

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }  
    }
}
