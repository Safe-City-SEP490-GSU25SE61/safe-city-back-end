using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class TransferReportDistrictRequestModel
    {
        public int NewDistrictId { get; set; }
        public string? Note { get; set; }
    }
}
