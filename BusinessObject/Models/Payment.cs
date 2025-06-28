using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int SubscriptionId { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "PayOs";
        public string Status { get; set; } = "Pending";

        public string TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }

        public Account User { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
        public PayosTransaction PayosTransaction { get; set; } = null!;
    }

}
