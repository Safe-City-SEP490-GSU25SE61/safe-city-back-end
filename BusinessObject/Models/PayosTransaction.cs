using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("payos_transaction")]
    public class PayosTransaction
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("payment_id")]
        public Guid PaymentId { get; set; }

        [Column("order_code")]
        public string OrderCode { get; set; }

        [Column("qr_code_url")]
        public string QrCodeUrl { get; set; }

        [Column("deep_link_url")]
        public string DeeplinkUrl { get; set; }

        [Column("exprired_at")]
        public DateTime ExpiredAt { get; set; }

        [Column("status")]
        public string Status { get; set; } = "INIT";

        [Column("webhook_received_at")]
        public DateTime? WebhookReceivedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Payment Payment { get; set; } = null!;
    }

}
