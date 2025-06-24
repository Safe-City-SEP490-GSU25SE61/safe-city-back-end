using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class CurrentSubscriptionResponseModel
    {
        public string PackageName { get; set; } = string.Empty;
        public string RemainingTime { get; set; } = string.Empty;
    }
}
