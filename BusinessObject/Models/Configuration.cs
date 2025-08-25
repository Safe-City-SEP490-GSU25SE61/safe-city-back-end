using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("configuration")]
    public class Configuration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("key")]
        public string Key { get; set; }

        [Column("value")]
        public string? Value { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [NotMapped]
        public int? ValueAsNumber
        {
            get => int.TryParse(Value, out var number) ? number : null;
            set => Value = value?.ToString();
        }

        [NotMapped]
        public bool? ValueAsBool
        {
            get => bool.TryParse(Value, out var result) ? result : null;
            set => Value = value?.ToString().ToLower();
        }
    }
}
