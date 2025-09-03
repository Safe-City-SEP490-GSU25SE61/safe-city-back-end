using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class PaymentStatusResponse
    {
        public string PackageName { get; set; }
        public decimal Amount { get; set; }
        public string? PaidAt { get; set; }
        public string OrderCode { get; set; }
        public string Status { get; set; }
    }
}
