using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ReportStatisticsResponse
    {
        public int TotalReports { get; set; }

        public Dictionary<string, int> ReportsByStatus { get; set; }

        public Dictionary<string, int> ReportsByCommune { get; set; }

        public string TopCommuneName { get; set; }

        public int? TopCommuneCount { get; set; }
        public int VisibleReports { get; set; }
        public int HiddenReports { get; set; }

        public Dictionary<string, int> ReportsByType { get; set; }

        public Dictionary<string, Dictionary<string, int>> ReportsBySubType { get; set; }
    }

}
