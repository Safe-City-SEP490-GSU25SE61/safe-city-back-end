using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupedPackageChangeDTO
    {
        public DateTime ChangedAt { get; set; }
        public DateTime EffectiveStart { get; set; }     
        public DateTime? EffectiveEnd { get; set; }
        public List<PackageChangeDetailDTO> Changes { get; set; }
    }

    public class PackageChangeDetailDTO
    {
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}

