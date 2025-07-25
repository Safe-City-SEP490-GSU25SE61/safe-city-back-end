using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class GroupedReportResponseModel
    {
        public ReportResponseModel MainReport { get; set; }
        public List<ReportResponseModel>? RelatedReports { get; set; } = new();
    }

}
