using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class AssignOfficerHistoryDTO
    {
        public Guid AccountId { get; set; }
        public int? OldDistrictId { get; set; }
        public int NewDistrictId { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
