using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class MapReportDetailsPolygonDTO
    {
        public string? Polygon { get; set; }
        public IEnumerable<MapReportDetailDTO> Reports { get; set; } = new List<MapReportDetailDTO>();
    }

    public class MapPointDTO
    {
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }

    public class MapReportDetailsDTO
    {
        public MapPointDTO? Point { get; set; }
        public List<MapReportDetailDTO> Reports { get; set; } = new();
    }

}
