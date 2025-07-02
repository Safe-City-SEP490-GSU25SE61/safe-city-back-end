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

    }
}
