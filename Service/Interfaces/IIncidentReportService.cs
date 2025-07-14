using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IIncidentReportService
    {
        Task<ReportResponseModel> CreateAsync(CreateReportRequestModel model, Guid userId);
        Task<ReportResponseModel> GetByIdAsync(Guid id);
        Task<IEnumerable<ReportResponseModel>> GetAllAsync();
        Task<ReportResponseModel> UpdateStatusAsync(Guid id, UpdateReportStatusRequestModel model, Guid officerId);
        Task<ReportResponseModel> AddNoteAsync(Guid id, AddInternalNoteRequestModel model, Guid officerId);
        Task<ReportResponseModel> CancelAsync(Guid reportId, Guid userId, string? reason = null);
        Task<IEnumerable<ReportResponseModel>> GetReportsByOfficerDistrictAsync(Guid officerId);
        Task<IEnumerable<ReportResponseModel>> GetFilteredReportsByOfficerAsync(Guid officerId, string? range, string? status, string? wardName = null, bool includeRelated = false);
        Task<IEnumerable<CitizenReportResponseModel>> GetFilteredReportsByCitizenAsync(Guid citizenId, string? range, string? status);
        Task<ReportResponseModel> TransferDistrictAsync(Guid reportId, TransferReportDistrictRequestModel model, Guid officerId);
    }
}

