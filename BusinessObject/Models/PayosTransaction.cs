using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class PayosTransaction
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public string OrderCode { get; set; }
        public string QrCodeUrl { get; set; }
        public string DeeplinkUrl { get; set; }

        public DateTime ExpiredAt { get; set; }

        public string Status { get; set; } = "INIT";
        public DateTime? WebhookReceivedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Payment Payment { get; set; } = null!;
    }

}
