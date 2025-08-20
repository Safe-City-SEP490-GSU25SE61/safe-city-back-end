using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class UpdateEscortGroupSettingsDTO
    {
        public string groupCode { get; set; }
        public bool AutoApprove { get; set; }
        public bool ReceiveRequest { get; set; }
    }
}
