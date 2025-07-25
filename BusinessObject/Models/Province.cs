using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("province")]
    public class Province
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [Column("note")]
        public string Note { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<Commune> Communes { get; set; }
    }
}
