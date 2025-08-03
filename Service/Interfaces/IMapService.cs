using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMapService
    {
        Task<IEnumerable<MapCommuneDTO>> GetAllCommunePolygonsAsync();
        Task<MapReportResponse> GetReportsForMapAsync(int communeId, string? type, string? range);

        Task<MapReportDetailDTO?> GetReportDetailForMapAsync(Guid reportId);

    }

}
