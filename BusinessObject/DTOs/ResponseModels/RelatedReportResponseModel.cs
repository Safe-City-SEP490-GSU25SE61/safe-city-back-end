using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class RelatedReportResponseModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
