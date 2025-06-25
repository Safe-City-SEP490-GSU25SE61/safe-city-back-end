using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    [Table("package_change_history")]
    public class PackageChangeHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("package_id")]
        public int PackageId { get; set; }

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
