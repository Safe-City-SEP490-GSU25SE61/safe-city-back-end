using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class BlogModerationResult
    {
        public bool IsApproved { get; set; }
        public bool Politeness { get; set; }
        public bool NoAntiState { get; set; }
        public bool PositiveMeaning { get; set; }
        public bool TypeRequirement { get; set; }
        public string Reasoning { get; set; } = string.Empty;
    }

}
