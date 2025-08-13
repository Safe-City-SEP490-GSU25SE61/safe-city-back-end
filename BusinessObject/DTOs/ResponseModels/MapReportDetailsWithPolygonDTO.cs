using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class MapReportDetailsWithPolygonDTO
    {
        public string? Polygon { get; set; }
        public IEnumerable<MapReportDetailDTO> Reports { get; set; } = new List<MapReportDetailDTO>();
    }

}
