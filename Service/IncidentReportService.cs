using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using BusinessObject.Events;
using BusinessObject.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;

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
        private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/jpg", "image/gif", "image/webp" };
        private static readonly string[] AllowedVideoTypes = { "video/mp4", "video/x-matroska", "video/quicktime", "video/webm" };
        private static ICommuneRepository _communeRepo;
        private readonly IMediator _mediator;
        private static readonly string[] ValidRanges = { "day", "week", "month", "year" };
        private static readonly string[] ValidRangesStatistic = { "week", "month", "quarter" };
        private static readonly string[] ValidStatuses = { "pending", "verified", "closed", "malicious","solved" };
        private static readonly string[] ValidCitizenStatuses = { "pending", "verified", "closed", "malicious", "solved", "cancelled" };
        private readonly IPointHistoryService _pointHistory;
        public IncidentReportService(IIncidentReportRepository reportRepo, INoteRepository noteRepo, IFirebaseStorageService storageService, IAccountRepository accountRepo, IAchievementRepository achievementRepo, IConfiguration configuration, ICommuneRepository communeRepo, IMediator mediator, IPointHistoryService pointHistory)
        {
            _reportRepo = reportRepo;
            _noteRepo = noteRepo;
            _storageService = storageService;
            _accountRepo = accountRepo;
            _achievementRepo = achievementRepo;
            _configuration = configuration;
            _communeRepo = communeRepo;
            _mediator = mediator;
            _pointHistory = pointHistory;
        }

        public async Task<ReportResponseModel> CreateAsync(CreateReportRequestModel model, Guid userId)
        {
            var account = await _accountRepo.GetByIdAsync(userId);
            if (account != null && account.ReputationPoint <= 0)
            {
                throw new InvalidOperationException("Tài khoản của bạn đã mất uy tín và không thể gửi báo cáo.");
            }

            if ((model.Images != null && model.Images.Any()) && model.Video != null)
            {
                throw new InvalidOperationException("Chỉ được phép gửi ảnh hoặc video, không được gửi cả hai.");
            }

            List<string> uploadedImageUrls = new();
            if (model.Images != null && model.Images.Any())
            {
                if (model.Images.Count > 3)
                    throw new InvalidOperationException("Chỉ được phép tải lên tối đa 3 hình ảnh.");

                foreach (var image in model.Images)
                {
                    if (!AllowedImageTypes.Contains(image.ContentType.ToLower()))
                        throw new InvalidOperationException($"Định dạng hình ảnh không hợp lệ: {image.FileName}");

                    var url = await _storageService.UploadFileAsync(image, "incident-report");
                    uploadedImageUrls.Add(url);
                }
            }
            string? uploadedVideoUrl = null;
            if (model.Video != null)
            {
                if (!AllowedVideoTypes.Contains(model.Video.ContentType.ToLower()))
                    throw new InvalidOperationException($"Định dạng video không hợp lệ: {model.Video.FileName}");

                uploadedVideoUrl = await _storageService.UploadFileAsync(model.Video, "incident-video");
            }

            int? communeId = null;
            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                var communeName = await ExtractCommuneName(model.Address);
                if (!string.IsNullOrEmpty(communeName))
                {
                    var commune = await _communeRepo.GetByNameAsync(communeName);
                    if (commune != null)
                    {
                        communeId = commune.Id;
                    }
                }
            }

            if (communeId == null)
            {
                throw new InvalidOperationException("This ward is not within the supported area.");
            }



            if (!Enum.IsDefined(typeof(IncidentType), model.Type))
            {
                var validTypes = IncidentTypeHelper.GetAllDisplayValues()
                    .Select(t => t.DisplayName)
                    .ToList();

                throw new InvalidOperationException($"Loại sự cố không hợp lệ. Các loại hợp lệ gồm: {string.Join(", ", validTypes)}");
            }




            if (model.PriorityLevel == null)
                throw new InvalidOperationException("Bạn cần chọn mức độ ưu tiên.");


            if (model.OccurredAt > DateTime.UtcNow || model.OccurredAt < DateTime.UtcNow.AddDays(-1))
                throw new InvalidOperationException("Thời gian xảy ra sự cố phải nằm trong 24 giờ gần nhất.");


            var report = new IncidentReport
            {
                UserId = userId,
                Type = model.Type,
                Description = model.Description,
                Lat = model.Lat,
                Lng = model.Lng,
                Address = model.Address,
                IsAnonymous = model.IsAnonymous,
                Status = "pending",
                OccurredAt = model.OccurredAt.ToUniversalTime(),
                CreatedAt = DateTime.UtcNow,
                ImageUrls = uploadedImageUrls.Any() ? System.Text.Json.JsonSerializer.Serialize(uploadedImageUrls) : null,
                VideoUrl = uploadedVideoUrl,
                CommuneId = communeId,
                PriorityLevel = model.PriorityLevel,
                IsVisibleOnMap = true,
            };
            if (string.IsNullOrWhiteSpace(model.SubCategory))
                throw new InvalidOperationException("Bạn cần chọn phân loại chi tiết.");

            var sub = model.SubCategory.Trim();
            switch (model.Type)
            {
                case IncidentType.Traffic:
                    if (!Enum.TryParse<TrafficSubCategory>(sub, true, out var traffic))
                        throw new InvalidOperationException("Phân loại chi tiết không hợp lệ cho loại Giao thông.");
                    report.TrafficSubCategory = traffic;
                    break;

                case IncidentType.Security:
                    if (!Enum.TryParse<SecuritySubCategory>(sub, true, out var security))
                        throw new InvalidOperationException("Phân loại chi tiết không hợp lệ cho loại An ninh.");
                    report.SecuritySubCategory = security;
                    break;

                case IncidentType.Infrastructure:
                    if (!Enum.TryParse<InfrastructureSubCategory>(sub, true, out var infra))
                        throw new InvalidOperationException("Phân loại chi tiết không hợp lệ cho loại Cơ sở hạ tầng.");
                    report.InfrastructureSubCategory = infra;
                    break;

                case IncidentType.Environment:
                    if (!Enum.TryParse<EnvironmentSubCategory>(sub, true, out var env))
                        throw new InvalidOperationException("Phân loại chi tiết không hợp lệ cho loại Môi trường.");
                    report.EnvironmentSubCategory = env;
                    break;

                case IncidentType.Other:
                    if (!Enum.TryParse<OtherSubCategory>(sub, true, out var other))
                        throw new InvalidOperationException("Phân loại chi tiết không hợp lệ cho loại Khác.");
                    report.OtherSubCategory = other;
                    break;
            }

            await _reportRepo.CreateAsync(report);

            var created = await _reportRepo.GetByIdAsync(report.Id);
            return ToResponseModel(created!);
        }

        private string? ExtractDistrictName(string address)
        {
            var tokens = address.Split(',');
            foreach (var token in tokens.Reverse())
            {
                var trimmed = token.Trim().ToLower();
                if (trimmed.StartsWith("quận") || trimmed.StartsWith("huyện") || trimmed == "thủ đức" || trimmed.StartsWith("thành phố thủ đức"))
                {
                    
                    return CultureInfo.GetCultureInfo("vi-VN").TextInfo.ToTitleCase(trimmed);
                }
            }
            return null;
        }

        private async Task<string?> ExtractCommuneName(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;

            var tokens = address.Split(',').Select(t => t.Trim().ToLowerInvariant()).Reverse();

            var allCommunes = await _communeRepo.GetAllAsync();
            var normalizedCommunes = allCommunes
                .Select(c => new
                {
                    Id = c.Id,
                    Original = c.Name,
                    Normalized = c.Name.ToLowerInvariant()
                        .Replace("phường ", "")
                        .Replace("xã ", "")
                        .Replace("thị trấn ", "")
                        .Trim()
                })
                .ToList();

            foreach (var token in tokens)
            {
                var cleaned = token
                    .Replace("p.", "")
                    .Replace("ph.", "")
                    .Replace("x.", "")
                    .Replace("tt.", "")
                    .Replace(".", "")
                    .Replace(":", "")
                    .Trim();

                var normalized = cleaned
                    .Replace("phường ", "")
                    .Replace("xã ", "")
                    .Replace("thị trấn ", "")
                    .Trim();

                var match = normalizedCommunes.FirstOrDefault(c => c.Normalized == normalized);
                if (match != null)
                {
                    return match.Original; 
                }
            }

            return null;
        }




        private string? ExtractWardName(string address)
        {
            var tokens = address.Split(',').Select(t => t.Trim()).ToList();

            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                var trimmed = tokens[i].ToLower();

                if (trimmed.StartsWith("phường ") || trimmed.StartsWith("xã "))
                {
                    return CultureInfo.GetCultureInfo("vi-VN").TextInfo.ToTitleCase(tokens[i]);
                }

                if (trimmed.StartsWith("p.") || trimmed.StartsWith("p. ") || trimmed.StartsWith("ph."))
                {
                    var wardName = trimmed.Replace("p.", "phường").Replace("ph.", "phường");
                    return CultureInfo.GetCultureInfo("vi-VN").TextInfo.ToTitleCase(wardName);
                }
            
                if (trimmed.StartsWith("quận") || trimmed.StartsWith("huyện") || trimmed == "thủ đức")
                {
                    if (i - 1 >= 0)
                    {
                        return CultureInfo.GetCultureInfo("vi-VN").TextInfo.ToTitleCase(tokens[i - 1]);
                    }
                }
            }

            return null;
        }




        private string ExtractStreetName(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return string.Empty;

            var parts = address.Split(',');
            if (parts.Length == 0) return string.Empty;

            var firstSegment = parts[0].Trim(); 
            var tokens = firstSegment.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var houseNumberPattern = @"^\d+[\w/.-]*$";
            int tokenIndex = 0;

            
            if (tokens.Length >= 2 && Regex.IsMatch(tokens[0], houseNumberPattern))
            {
                tokenIndex = 1;
            }

            
            if (tokens.Length > tokenIndex && IsDuongToken(tokens[tokenIndex]))
            {
                tokenIndex++;
            }

            
            var streetTokens = tokens.Skip(tokenIndex);
            return string.Join(" ", streetTokens).ToLower();
        }

        private bool IsDuongToken(string token)
        {
            var normalized = token.ToLower().Trim('.', ':');
            return normalized == "đường" || normalized == "đ" || normalized == "đg";
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

            var allowedTransitionsFrom = new Dictionary<string, string[]>
            {
                { "pending", new[] { "verified", "closed", "malicious", "solved" } },
                { "pending", new[] { "solved" } },
                { "verified", new[] { "solved" } },                  
            };

            if (!allowedTransitionsFrom.TryGetValue(report.Status, out var nextStatuses) ||
                !nextStatuses.Contains(model.Status))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ {report.Status} sang {model.Status}.");
            }

            report.Status = model.Status;
            report.VerifiedBy = officerId;
            if (!string.IsNullOrWhiteSpace(model.Message))
            {
                report.StatusMessage = model.Message;
            }

            await _reportRepo.UpdateAsync(report);


            if (model.Status == "verified")
            {
                var account = await _accountRepo.GetByIdAsync(report.UserId);
                if (account != null)
                {
                    int rewardPoint = _configuration.GetValue<int>("Reward:VerifiedReportPoint", 100);
                    account.TotalPoint += rewardPoint;
                    await _accountRepo.UpdateOfficerAsync(account);

                    await _pointHistory.LogAsync(
                        userId: report.UserId,
                        actorId: officerId,
                        sourceType: "incident_report",
                        sourceId: report.Id.ToString(),
                        action: "report_verified",
                        pointsDelta: rewardPoint,
                        reputationDelta: 0,
                        note: model.Message
                    );
                }
            }
            if (model.Status == "malicious")
            {
                var reporter = await _accountRepo.GetByIdAsync(report.UserId);
                if (reporter != null)
                {
                    reporter.ReputationPoint = Math.Max(0, reporter.ReputationPoint - 1);
                    await _accountRepo.UpdateOfficerAsync(reporter);

                    await _pointHistory.LogAsync(
                        userId: report.UserId,
                        actorId: officerId,
                        sourceType: "incident_report",
                        sourceId: report.Id.ToString(),
                        action: "report_malicious_penalty",
                        pointsDelta: 0,
                        reputationDelta: -1,
                        note: model.Message
                    );
                }
            }

            var updated = await _reportRepo.GetByIdAsync(id);
            return ToResponseModel(updated!);
        }

        public async Task<ReportResponseModel> UpdateVisibilityAsync(Guid id, UpdateReportVisibilityRequestModel model)
        {
            var report = await _reportRepo.GetByIdAsync(id);
            if (report == null) throw new KeyNotFoundException("Report not found");

            report.IsVisibleOnMap = model.IsVisibleOnMap;

            await _reportRepo.UpdateAsync(report);

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
            //var tz = TZConvert.GetTimeZoneInfo("SE Asia Standard Time");
            return new ReportResponseModel
            {
                Id = report.Id,
                Type = IncidentTypeHelper.GetAllDisplayValues()
                .FirstOrDefault(t => t.Value == report.Type.ToString()).DisplayName ?? "Không xác định",
                Description = report.Description,
                Lat = report.Lat,
                Lng = report.Lng,
                Address = report.Address,
                Status = report.Status,
                IsAnonymous = report.IsAnonymous,
                IsVisibleOnMap = report.IsVisibleOnMap,
                OccurredAt = DateTimeHelper.ToVietnamTime(report.OccurredAt),
                CreatedAt = DateTimeHelper.ToVietnamTime(report.CreatedAt),
                VerifiedByName = report.Verifier?.FullName,
                StatusMessage = report.StatusMessage,
                UserName = report.IsAnonymous ? null : report.User.FullName,
                CommuneName = report.Commune?.Name,
                Notes = report.Notes.Select(n =>$"{n.CreatedAt:yyyy-MM-dd HH:mm} [{n.Officer?.FullName ?? "Không xác định"}]: {n.Content}").ToList(),
                ImageUrls = string.IsNullOrEmpty(report.ImageUrls)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(report.ImageUrls),
                VideoUrl = report.VideoUrl,
                PriorityLevel = report.PriorityLevel?.ToString(),
                SubCategory = report.Type switch
                {
                    IncidentType.Traffic => IncidentTypeHelper
                        .GetDisplayValues<TrafficSubCategory>()
                        .FirstOrDefault(x => x.Value == report.TrafficSubCategory?.ToString()).DisplayName,

                    IncidentType.Security => IncidentTypeHelper
                        .GetDisplayValues<SecuritySubCategory>()
                        .FirstOrDefault(x => x.Value == report.SecuritySubCategory?.ToString()).DisplayName,

                    IncidentType.Infrastructure => IncidentTypeHelper
                        .GetDisplayValues<InfrastructureSubCategory>()
                        .FirstOrDefault(x => x.Value == report.InfrastructureSubCategory?.ToString()).DisplayName,

                    IncidentType.Environment => IncidentTypeHelper
                        .GetDisplayValues<EnvironmentSubCategory>()
                        .FirstOrDefault(x => x.Value == report.EnvironmentSubCategory?.ToString()).DisplayName,

                    IncidentType.Other => IncidentTypeHelper
                        .GetDisplayValues<OtherSubCategory>()
                        .FirstOrDefault(x => x.Value == report.OtherSubCategory?.ToString()).DisplayName,

                    _ => null
                }

        };
        }

        public async Task<ReportResponseModel> CancelAsync(Guid reportId, Guid userId, string? reason = null)
        {
            var report = await _reportRepo.GetByIdAsync(reportId);
            if (report == null || report.UserId != userId)
                throw new KeyNotFoundException("Không tìm thấy report hoặc bạn không có quyền huỷ.");

            if (report.Status != "pending")
                throw new InvalidOperationException("Chỉ có thể huỷ report khi trạng thái là 'pending'.");
            var timeSinceCreated = DateTime.UtcNow - report.CreatedAt;
            if (timeSinceCreated.TotalMinutes > 5)
                throw new InvalidOperationException("Bạn chỉ có thể huỷ báo cáo trong vòng 5 phút sau khi tạo.");
            report.Status = "cancelled";
            if (!string.IsNullOrWhiteSpace(reason))
            {
                report.StatusMessage = reason;
            }
            await _reportRepo.UpdateAsync(report);
            var updated = await _reportRepo.GetByIdAsync(reportId);
            return ToResponseModel(updated!);
        }
        public async Task<IEnumerable<ReportResponseModel>> GetReportsByOfficerDistrictAsync(Guid officerId)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null)
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            if (officer.Role?.Name?.ToLower() != "officer")
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập chức năng này.");

            if (officer.CommuneId == null)
                throw new InvalidOperationException("Bạn chưa được gán khu vực quản lý.");


            var allReports = await _reportRepo.GetAllAsync();
            var filtered = allReports
                .Where(r => r.CommuneId == officer.CommuneId)
                .Select(ToResponseModel);

            return filtered;
        }

        public async Task<IEnumerable<GroupedReportResponseModel>> GetFilteredReportsByOfficerAsync(Guid officerId, string? range, string? status, bool includeRelated = false, string? sort = null, PriorityLevel? priorityFilter = null)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null)
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            if (officer.Role?.Name?.ToLower() != "officer")
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập chức năng này.");

            if (officer.CommuneId == null)
                throw new InvalidOperationException("Bạn chưa được gán khu vực quản lý.");

            var allReports = (await _reportRepo.GetAllAsync())
                .Where(r => r.CommuneId == officer.CommuneId)
                .ToList();

            if (!string.IsNullOrEmpty(status))
            {
                if (!ValidStatuses.Contains(status.ToLower()))
                    throw new ArgumentException($"Giá trị 'status' không hợp lệ. Hợp lệ: {string.Join(", ", ValidStatuses)}");
                allReports = allReports.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                allReports = allReports.Where(r => !r.Status.Equals("cancelled", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(range))
            {
                if (!ValidRanges.Contains(range.ToLower()))
                    throw new ArgumentException($"Giá trị 'range' không hợp lệ. Hợp lệ: {string.Join(", ", ValidRanges)}");
                DateTime fromDate = range.ToLower() switch
                {
                    "day" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "year" => DateTime.UtcNow.AddYears(-1),
                    _ => DateTime.MinValue
                };
                allReports = allReports.Where(r => r.CreatedAt >= fromDate).ToList();
            }
            if (priorityFilter.HasValue)
            {
                allReports = allReports.Where(r => r.PriorityLevel == priorityFilter).ToList();
            }
            var validSorts = new[] { "newest", "oldest", "urgent" };
            if (string.IsNullOrWhiteSpace(sort) || !validSorts.Contains(sort.ToLower()))
            {
                throw new ArgumentException($"Giá trị 'sort' không hợp lệ. Hãy chọn một trong: {string.Join(", ", validSorts)}");
            }
            allReports = sort.ToLower() switch
            {
                "oldest" => allReports.OrderBy(r => r.CreatedAt).ToList(),
                "urgent" => allReports.OrderByDescending(r => r.PriorityLevel).ThenByDescending(r => r.CreatedAt).ToList(),
                "newest" => allReports.OrderByDescending(r => r.CreatedAt).ToList(),
                _ => allReports.OrderByDescending(r => r.CreatedAt).ToList()
            };


            var results = new List<GroupedReportResponseModel>();
            var visited = new HashSet<Guid>();


            foreach (var report in allReports)
            {
                if (visited.Contains(report.Id)) continue;

                var response = ToResponseModel(report);

                if (includeRelated)
                {
                    var streetName = ExtractStreetName(report.Address);
                    var related = allReports
                        .Where(r =>
                            r.Id != report.Id &&
                            !visited.Contains(r.Id) &&
                            r.Type == report.Type &&
                            IsSameSubCategory(report, r) &&
                            ExtractStreetName(r.Address) == streetName &&
                            Math.Abs((r.CreatedAt - report.CreatedAt).TotalMinutes) <= 15 &&
                            report.Lat.HasValue && report.Lng.HasValue &&
                            r.Lat.HasValue && r.Lng.HasValue &&
                            CalculateDistanceInMeters((double)report.Lat.Value, (double)report.Lng.Value, (double)r.Lat.Value, (double)r.Lng.Value) <= 300
                        )
                        .ToList()
                        .OrderByDescending(r => r.CreatedAt)
                        .ToList();

                    var relatedResponses = related.Select(ToResponseModel).ToList();

                    foreach (var r in related) visited.Add(r.Id);

                    results.Add(new GroupedReportResponseModel
                    {
                        MainReport = response,
                        RelatedReports = relatedResponses
                    });
                }
                else
                {
                    results.Add(new GroupedReportResponseModel
                    {
                        MainReport = response
                    });
                }

                visited.Add(report.Id);
            }

            return results;
        }
        private double CalculateDistanceInMeters(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371000; 
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLng = (lng2 - lng1) * Math.PI / 180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private bool IsSameSubCategory(IncidentReport a, IncidentReport b)
        {
            return a.Type switch
            {
                IncidentType.Traffic => a.TrafficSubCategory == b.TrafficSubCategory,
                IncidentType.Security => a.SecuritySubCategory == b.SecuritySubCategory,
                IncidentType.Infrastructure => a.InfrastructureSubCategory == b.InfrastructureSubCategory,
                IncidentType.Environment => a.EnvironmentSubCategory == b.EnvironmentSubCategory,
                IncidentType.Other => a.OtherSubCategory == b.OtherSubCategory,
                _ => true
            };
        }


        public async Task<IEnumerable<CitizenReportResponseModel>> GetFilteredReportsByCitizenAsync(Guid citizenId, string? range, string? status, string? sort, PriorityLevel? priorityFilter = null, string? communeName = null)
        {
            var reports = (await _reportRepo.GetAllAsync())
                .Where(r => r.UserId == citizenId);

            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "null" && status.ToLower() != "undefined")
            {
                var normalizedStatus = status.Trim().ToLower();
                if (!ValidCitizenStatuses.Contains(normalizedStatus))
                    throw new ArgumentException($"Giá trị 'status' không hợp lệ. Hợp lệ: {string.Join(", ", ValidCitizenStatuses)}");

                reports = reports.Where(r => r.Status.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(range) && range.ToLower() != "null" && range.ToLower() != "undefined")
            {
                var normalizedRange = range.Trim().ToLower();
                if (!ValidRanges.Contains(normalizedRange))
                    throw new ArgumentException($"Giá trị 'range' không hợp lệ. Hợp lệ: {string.Join(", ", ValidRanges)}");

                DateTime fromDate = normalizedRange switch
                {
                    "day" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "year" => DateTime.UtcNow.AddYears(-1),
                    _ => DateTime.MinValue
                };

                reports = reports.Where(r => r.CreatedAt >= fromDate);
            }
            if (priorityFilter.HasValue)
            {
                reports = reports.Where(r => r.PriorityLevel == priorityFilter);
            }

            if (!string.IsNullOrWhiteSpace(communeName))
            {
                var normalized = communeName.Trim().ToLowerInvariant();
                reports = reports.Where(r => r.Commune != null &&
                                             r.Commune.Name.ToLowerInvariant().Contains(normalized));
            }

            var validSorts = new[] { "newest", "oldest", "urgent" };
            if (string.IsNullOrWhiteSpace(sort) || !validSorts.Contains(sort.ToLower()))
            {
                throw new ArgumentException($"Giá trị 'sort' không hợp lệ. Hãy chọn một trong: {string.Join(", ", validSorts)}");
            }

            reports = sort.ToLower() switch
            {
                "oldest" => reports.OrderBy(r => r.CreatedAt),
                "urgent" => reports.OrderByDescending(r => r.PriorityLevel).ThenByDescending(r => r.CreatedAt),
                "newest" => reports.OrderByDescending(r => r.CreatedAt),
                _ => reports.OrderByDescending(r => r.CreatedAt)
            };

            return reports.Select(r => new CitizenReportResponseModel
            {
                Id = r.Id,
                Type = IncidentTypeHelper.GetAllDisplayValues()
                .FirstOrDefault(t => t.Value == r.Type.ToString()).DisplayName ?? "Không xác định",
                Description = r.Description,
                Address = r.Address,
                Status = r.Status,
                StatusMessage = r.StatusMessage,
                IsAnonymous = r.IsAnonymous,
                OccurredAt = DateTimeHelper.ToVietnamTime(r.OccurredAt),
                CreatedAt = DateTimeHelper.ToVietnamTime(r.CreatedAt),
                ImageUrls = string.IsNullOrEmpty(r.ImageUrls)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(r.ImageUrls),
                VideoUrl = r.VideoUrl,
                PriorityLevel = r.PriorityLevel?.ToString(),
                SubCategory = r.Type switch
                {
                    IncidentType.Traffic => IncidentTypeHelper
                        .GetDisplayValues<TrafficSubCategory>()
                        .FirstOrDefault(x => x.Value == r.TrafficSubCategory?.ToString()).DisplayName,

                    IncidentType.Security => IncidentTypeHelper
                        .GetDisplayValues<SecuritySubCategory>()
                        .FirstOrDefault(x => x.Value == r.SecuritySubCategory?.ToString()).DisplayName,

                    IncidentType.Infrastructure => IncidentTypeHelper
                        .GetDisplayValues<InfrastructureSubCategory>()
                        .FirstOrDefault(x => x.Value == r.InfrastructureSubCategory?.ToString()).DisplayName,

                    IncidentType.Environment => IncidentTypeHelper
                        .GetDisplayValues<EnvironmentSubCategory>()
                        .FirstOrDefault(x => x.Value == r.EnvironmentSubCategory?.ToString()).DisplayName,

                    IncidentType.Other => IncidentTypeHelper
                        .GetDisplayValues<OtherSubCategory>()
                        .FirstOrDefault(x => x.Value == r.OtherSubCategory?.ToString()).DisplayName,

                    _ => null
                }
            });
        }
        public async Task<ReportResponseModel> TransferDistrictAsync(Guid reportId, TransferReportDistrictRequestModel model, Guid officerId)
        {
            var report = await _reportRepo.GetByIdAsync(reportId);
            if (report == null)
                throw new KeyNotFoundException("Không tìm thấy báo cáo.");

            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null || officer.Role?.Name?.ToLower() != "officer")
                throw new UnauthorizedAccessException("Chỉ cán bộ mới có quyền chuyển khu vực.");

            var newDistrict = await _communeRepo.GetByIdAsync(model.NewDistrictId);
            if (newDistrict == null)
                throw new InvalidOperationException("Khu vực mới không hợp lệ.");

            if (report.CommuneId == model.NewDistrictId)
                throw new InvalidOperationException("Báo cáo đã thuộc khu vực này.");

            var oldCommune = await _communeRepo.GetByIdAsync(report.CommuneId ?? 0);

            report.CommuneId = model.NewDistrictId;

            report.VerifiedBy = officerId;

            await _reportRepo.UpdateAsync(report);

           
            var transferNote = $"Chuyển khu vực từ {(oldCommune?.Name ?? "null")} sang {newDistrict.Name}.";
            if (!string.IsNullOrWhiteSpace(model.Note))
            {
                transferNote += $" Ghi chú: {model.Note}";
            }

            var note = new Note
            {
                OfficerId = officerId,
                ReportId = reportId,
                Content = transferNote,
                CreatedAt = DateTime.UtcNow
            };
            await _noteRepo.CreateAsync(note);


            var updated = await _reportRepo.GetByIdAsync(reportId);
            return ToResponseModel(updated!);
        }
        public async Task<IEnumerable<GroupedReportResponseModel>> GetFilteredReportsForAdminAsync(string? range,string? status,bool includeRelated = false,string? sort = null,PriorityLevel? priorityFilter = null)
        {
            var allReports = (await _reportRepo.GetAllAsync()).ToList();

            if (!string.IsNullOrEmpty(status))
            {
                if (!ValidStatuses.Contains(status.ToLower()))
                    throw new ArgumentException($"Giá trị 'status' không hợp lệ. Hợp lệ: {string.Join(", ", ValidStatuses)}");

                allReports = allReports.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }


            if (!string.IsNullOrEmpty(range))
            {
                if (!ValidRanges.Contains(range.ToLower()))
                    throw new ArgumentException($"Giá trị 'range' không hợp lệ. Hợp lệ: {string.Join(", ", ValidRanges)}");

                DateTime fromDate = range.ToLower() switch
                {
                    "day" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "year" => DateTime.UtcNow.AddYears(-1),
                    _ => DateTime.MinValue
                };

                allReports = allReports.Where(r => r.CreatedAt >= fromDate).ToList();
            }

            if (priorityFilter.HasValue)
            {
                allReports = allReports.Where(r => r.PriorityLevel == priorityFilter).ToList();
            }

            var validSorts = new[] { "newest", "oldest", "urgent" };
            if (string.IsNullOrWhiteSpace(sort) || !validSorts.Contains(sort.ToLower()))
            {
                throw new ArgumentException($"Giá trị 'sort' không hợp lệ. Hãy chọn một trong: {string.Join(", ", validSorts)}");
            }

            allReports = sort.ToLower() switch
            {
                "oldest" => allReports.OrderBy(r => r.CreatedAt).ToList(),
                "urgent" => allReports.OrderByDescending(r => r.PriorityLevel).ThenByDescending(r => r.CreatedAt).ToList(),
                "newest" => allReports.OrderByDescending(r => r.CreatedAt).ToList(),
                _ => allReports.OrderByDescending(r => r.CreatedAt).ToList()
            };

            var results = new List<GroupedReportResponseModel>();
            var visited = new HashSet<Guid>();

            foreach (var report in allReports)
            {
                if (visited.Contains(report.Id)) continue;

                var response = ToResponseModel(report);

                if (includeRelated)
                {
                    var streetName = ExtractStreetName(report.Address);
                    var related = allReports
                        .Where(r =>
                            r.Id != report.Id &&
                            !visited.Contains(r.Id) &&
                            r.Type == report.Type &&
                            IsSameSubCategory(report, r) &&
                            ExtractStreetName(r.Address) == streetName &&
                            Math.Abs((r.CreatedAt - report.CreatedAt).TotalMinutes) <= 15 &&
                            report.Lat.HasValue && report.Lng.HasValue &&
                            r.Lat.HasValue && r.Lng.HasValue &&
                            CalculateDistanceInMeters((double)report.Lat.Value, (double)report.Lng.Value, (double)r.Lat.Value, (double)r.Lng.Value) <= 300
                        )
                        .ToList()
                        .OrderByDescending(r => r.CreatedAt)
                        .ToList();

                    var relatedResponses = related.Select(ToResponseModel).ToList();

                    foreach (var r in related) visited.Add(r.Id);

                    results.Add(new GroupedReportResponseModel
                    {
                        MainReport = response,
                        RelatedReports = relatedResponses
                    });
                }
                else
                {
                    results.Add(new GroupedReportResponseModel
                    {
                        MainReport = response
                    });
                }

                visited.Add(report.Id);
            }

            return results;
        }

        public async Task<ReportStatisticsResponse> GetSystemReportStatisticsAsync(string? range)
        {
            var allReports = await _reportRepo.GetAllAsync();
            var visibleCount = allReports.Count(r => r.IsVisibleOnMap);
            var hiddenCount = allReports.Count(r => !r.IsVisibleOnMap);
            if (!string.IsNullOrWhiteSpace(range))
            {
                if (!ValidRangesStatistic.Contains(range.ToLower()))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: week, month, quarter");

                DateTime fromDate = range.ToLower() switch
                {
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "quarter" => DateTime.UtcNow.AddMonths(-3),
                    _ => DateTime.UtcNow.AddDays(-7)
                };

                allReports = allReports
                    .Where(r => r.CreatedAt >= fromDate) 
                    .ToList();
            }

            var total = allReports.Count();

            var reportsByStatus = allReports
                .GroupBy(r => r.Status.ToLower())
                .ToDictionary(g => g.Key, g => g.Count());

            var reportsByCommune = allReports
                .Where(r => r.Commune != null)
                .GroupBy(r => r.Commune.Name)
                .ToDictionary(g => g.Key, g => g.Count());

            var topCommune = reportsByCommune
                .OrderByDescending(x => x.Value)
                .FirstOrDefault();

            var reportsByType = allReports
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => IncidentTypeHelper.GetAllDisplayValues()
                            .FirstOrDefault(t => t.Value == g.Key.ToString()).DisplayName ?? g.Key.ToString(),
                    g => g.Count());

            var reportsBySubType = new Dictionary<string, Dictionary<string, int>>();

            foreach (var typeGroup in allReports.GroupBy(r => r.Type))
            {
                var type = IncidentTypeHelper.GetAllDisplayValues()
                    .FirstOrDefault(t => t.Value == typeGroup.Key.ToString()).DisplayName ?? typeGroup.Key.ToString();

                Dictionary<string, int> subTypeCounts = typeGroup.Key switch
                {
                    IncidentType.Traffic => typeGroup
                        .GroupBy(r => r.TrafficSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<TrafficSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Security => typeGroup
                        .GroupBy(r => r.SecuritySubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<SecuritySubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Infrastructure => typeGroup
                        .GroupBy(r => r.InfrastructureSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<InfrastructureSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Environment => typeGroup
                        .GroupBy(r => r.EnvironmentSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<EnvironmentSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Other => typeGroup
                        .GroupBy(r => r.OtherSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<OtherSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    _ => new Dictionary<string, int>()
                };

                reportsBySubType[type] = subTypeCounts;
            }

            return new ReportStatisticsResponse
            {
                TotalReports = total,
                ReportsByStatus = reportsByStatus,
                ReportsByCommune = reportsByCommune,
                TopCommuneName = topCommune.Key ?? "N/A",
                TopCommuneCount = topCommune.Value,
                ReportsByType = reportsByType,
                ReportsBySubType = reportsBySubType,
                VisibleReports = visibleCount,
                HiddenReports = hiddenCount
            };
        }

        public async Task<ReportStatisticsResponse> GetOfficerStatisticsAsync(Guid officerId, string? range)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null)
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            if (officer.Role?.Name?.ToLower() != "officer")
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập chức năng này.");

            if (officer.CommuneId == null)
                throw new InvalidOperationException("Bạn chưa được gán khu vực quản lý.");

            var reports = (await _reportRepo.GetAllAsync())
                .Where(r => r.CommuneId == officer.CommuneId)
                .ToList();
            var visibleCount = reports.Count(r => r.IsVisibleOnMap);
            var hiddenCount = reports.Count(r => !r.IsVisibleOnMap);

            if (!string.IsNullOrWhiteSpace(range))
            {
                if (!ValidRangesStatistic.Contains(range.ToLower()))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: week, month, quarter");

                var fromDate = range.ToLower() switch
                {
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "quarter" => DateTime.UtcNow.AddMonths(-3),
                    _ => DateTime.UtcNow.AddDays(-7)
                };

                reports = reports.Where(r => r.CreatedAt >= fromDate).ToList();
            }

            var total = reports.Count;

            var reportsByStatus = reports
                .GroupBy(r => r.Status.ToLower())
                .ToDictionary(g => g.Key, g => g.Count());

            var reportsByType = reports
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => IncidentTypeHelper.GetAllDisplayValues()
                        .FirstOrDefault(t => t.Value == g.Key.ToString()).DisplayName ?? g.Key.ToString(),
                    g => g.Count());

            var reportsBySubType = new Dictionary<string, Dictionary<string, int>>();

            foreach (var typeGroup in reports.GroupBy(r => r.Type))
            {
                var type = IncidentTypeHelper.GetAllDisplayValues()
                    .FirstOrDefault(t => t.Value == typeGroup.Key.ToString()).DisplayName ?? typeGroup.Key.ToString();

                Dictionary<string, int> subTypeCounts = typeGroup.Key switch
                {
                    IncidentType.Traffic => typeGroup
                        .GroupBy(r => r.TrafficSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<TrafficSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Security => typeGroup
                        .GroupBy(r => r.SecuritySubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<SecuritySubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Infrastructure => typeGroup
                        .GroupBy(r => r.InfrastructureSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<InfrastructureSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Environment => typeGroup
                        .GroupBy(r => r.EnvironmentSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<EnvironmentSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    IncidentType.Other => typeGroup
                        .GroupBy(r => r.OtherSubCategory?.ToString() ?? "Unknown")
                        .ToDictionary(
                            g => IncidentTypeHelper.GetDisplayValues<OtherSubCategory>()
                                .FirstOrDefault(x => x.Value == g.Key).DisplayName ?? g.Key,
                            g => g.Count()),

                    _ => new Dictionary<string, int>()
                };

                reportsBySubType[type] = subTypeCounts;
            }

            return new ReportStatisticsResponse
            {
                TotalReports = total,
                ReportsByStatus = reportsByStatus,
                ReportsByCommune = null,
                TopCommuneName = null,
                TopCommuneCount = null,
                ReportsByType = reportsByType,
                ReportsBySubType = reportsBySubType,
                VisibleReports = visibleCount,
                HiddenReports = hiddenCount
            };
        }





    }

}
