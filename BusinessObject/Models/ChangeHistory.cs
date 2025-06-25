using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("change_history")]
    public class ChangeHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("entity_type")]
        public string EntityType { get; set; } 

        [Column("entity_id")]
        public string EntityId { get; set; } 

        [Column("field_name")]
        public string FieldName { get; set; } 

        [Column("old_value")]
        public string OldValue { get; set; }

        [Column("new_value")]
        public string NewValue { get; set; }

        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }
    }
}
