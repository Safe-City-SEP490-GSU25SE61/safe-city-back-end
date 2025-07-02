using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("subcription_id")]
        public int SubscriptionId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = "PayOs";

        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("transaction_code")]
        public string TransactionCode { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        public Account User { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
        public PayosTransaction PayosTransaction { get; set; } = null!;
    }

}
