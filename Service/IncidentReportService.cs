using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class IncidentReportService : IIncidentReportService
    {
        private readonly IIncidentReportRepository _reportRepo;
        private readonly INoteRepository _noteRepo;
        private readonly IFirebaseStorageService _storageService;

        public IncidentReportService(IIncidentReportRepository reportRepo, INoteRepository noteRepo, IFirebaseStorageService storageService)
        {
            _reportRepo = reportRepo;
            _noteRepo = noteRepo;
            _storageService = storageService;
        }

        public async Task<ReportResponseModel> CreateAsync(CreateReportRequestModel model, Guid userId)
        {
            List<string> uploadedImageUrls = new();
            if (model.Images != null && model.Images.Any())
            {
                foreach (var image in model.Images)
                {
                    var url = await _storageService.UploadFileAsync(image, "incident-report");
                    uploadedImageUrls.Add(url);
                }
            }

            var report = new IncidentReport
            {
                UserId = userId,
                Type = model.Type,
                Description = model.Description,
                Address = model.Address,
                IsAnonymous = model.IsAnonymous,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                ImageUrls = uploadedImageUrls.Any() ? System.Text.Json.JsonSerializer.Serialize(uploadedImageUrls) : null
            };

            await _reportRepo.CreateAsync(report);

            var created = await _reportRepo.GetByIdAsync(report.Id);
            return ToResponseModel(created!);
        }



        public async Task<IEnumerable<ReportResponseModel>> GetAllAsync()
        {
            var reports = await _reportRepo.GetAllAsync();
            return reports.Select(ToResponseModel);
        }

        public async Task<ReportResponseModel> GetByIdAsync(Guid id)
        {
            var report = await _reportRepo.GetByIdAsync(id);
            if (report == null) throw new KeyNotFoundException("Report not found");

            return ToResponseModel(report);
        }

        public async Task<ReportResponseModel> UpdateStatusAsync(Guid id, UpdateReportStatusRequestModel model, Guid officerId)
        {
            await _reportRepo.UpdateStatusAsync(id, model.Status, officerId);
            var updated = await _reportRepo.GetByIdAsync(id);
            return ToResponseModel(updated);
        }

        public async Task<ReportResponseModel> AddNoteAsync(Guid id, AddInternalNoteRequestModel model, Guid officerId)
        {
            var note = new Note
            {
                OfficerId = officerId,
                ReportId = id,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            };
            await _noteRepo.CreateAsync(note);
            var report = await _reportRepo.GetByIdAsync(id);
            return ToResponseModel(report);
        }


        private static ReportResponseModel ToResponseModel(IncidentReport report)
        {
            return new ReportResponseModel
            {
                Id = report.Id,
                Type = report.Type,
                Description = report.Description,
                Lat = report.Lat,
                Lng = report.Lng,
                Address = report.Address,
                Status = report.Status,
                IsAnonymous = report.IsAnonymous,
                CreatedAt = report.CreatedAt,
                VerifiedByName = report.Verifier?.FullName,
                UserName = report.IsAnonymous ? null : report.User.FullName,
                DistrictName = report.District?.Name,
                WardName = report.Ward?.Name,
                Notes = report.Notes.Select(n => $"[{n.CreatedAt:yyyy-MM-dd HH:mm}] {n.Officer.FullName}: {n.Content}").ToList(),
                ImageUrls = string.IsNullOrEmpty(report.ImageUrls)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(report.ImageUrls)

            };
        }
    }

}
