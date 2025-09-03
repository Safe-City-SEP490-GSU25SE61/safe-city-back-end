using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class PaymentHistoryResponseModel
    {
        public string OrderCode { get; set; } = default!;
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; } = default!;
        public string PaidAt { get; set; } = default!;
        public string PackageName { get; set; } = default!;
    }

}
