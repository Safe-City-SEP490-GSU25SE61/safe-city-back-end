using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly IAccountRepository _accountRepo;
        private readonly IAchievementRepository _achievementRepo;
        private readonly IConfiguration _configuration;


        public IncidentReportService(IIncidentReportRepository reportRepo, INoteRepository noteRepo, IFirebaseStorageService storageService, IAccountRepository accountRepo, IAchievementRepository achievementRepo, IConfiguration configuration)
        {
            _reportRepo = reportRepo;
            _noteRepo = noteRepo;
            _storageService = storageService;
            _accountRepo = accountRepo;
            _achievementRepo = achievementRepo;
            _configuration = configuration;
        }

        public async Task<ReportResponseModel> CreateAsync(CreateReportRequestModel model, Guid userId)
        {
            List<string> uploadedImageUrls = new();
            if (model.Images != null && model.Images.Any())
            {
                if (model.Images.Count > 3)
                    throw new InvalidOperationException("Chỉ được phép tải lên tối đa 3 hình ảnh.");

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
            var report = await _reportRepo.GetByIdAsync(id);
            if (report == null) throw new KeyNotFoundException("Report not found");
       
            var allowedStatuses = new[] { "verified", "rejected" };
            if (!allowedStatuses.Contains(model.Status.ToLower()))
                throw new InvalidOperationException("Invalid status. Only 'verified' or 'rejected' are allowed.");
       
            if (report.Status != "pending")
                throw new InvalidOperationException("Status can only be changed if current status is 'pending'.");
 
            await _reportRepo.UpdateStatusAsync(id, model.Status, officerId);

            
            if (model.Status == "verified")
            {
                var account = await _accountRepo.GetByIdAsync(report.UserId);
                if (account != null)
                {
                    int rewardPoint = _configuration.GetValue<int>("Reward:VerifiedReportPoint", 10);
                    account.TotalPoint += rewardPoint;

                    var allAchievements = await _achievementRepo.GetAllAsync();
                    var matched = allAchievements
                        .Where(a => a.MinPoint <= account.TotalPoint)
                        .OrderByDescending(a => a.MinPoint)
                        .FirstOrDefault();

                    if (matched != null && account.AchievementId != matched.Id)
                    {
                        account.AchievementId = matched.Id;
                    }

                    await _accountRepo.UpdateOfficerAsync(account);
                }
            }

            var updated = await _reportRepo.GetByIdAsync(id);
            return ToResponseModel(updated!);
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
