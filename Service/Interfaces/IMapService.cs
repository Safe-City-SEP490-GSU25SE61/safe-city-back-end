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

        Task<IEnumerable<MapReportDetailDTO>> GetReportDetailsForMapAsync(int communeId, string? type, string? range);
        Task<MapReportResponse> GetOfficerReportsForMapAsync(Guid officerId, string? type, string? range);
        Task<IEnumerable<MapReportDetailDTO>> GetOfficerReportDetailsForMapAsync(Guid officerId, string? type, string? range);
        Task<MapReportDetailsWithPolygonDTO> GetOfficerReportDetailsWithPolygonAsync(Guid officerId, string? type, string? range);
        Task<MapReportResponse> GetAdminReportsForMapAsync(int communeId, string? type, string? range);
        Task<MapReportDetailsWithPolygonDTO> GetAdminReportDetailsWithPolygonAsync(int communeId, string? type, string? range);

    }

}
