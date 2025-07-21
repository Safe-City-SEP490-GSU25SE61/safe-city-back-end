using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupedAssignOfficerChangeDTO
    {
        public DateTime ChangedAt { get; set; }
        public List<AssignOfficerChangeDTO> Changes { get; set; }
    }

    public class AssignOfficerChangeDTO
    {
        public string OldCommuneName { get; set; }
        public string NewCommuneName { get; set; }
    }
}

