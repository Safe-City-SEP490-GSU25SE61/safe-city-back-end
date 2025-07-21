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
        private static IDistrictRepository _districtRepo;
        private readonly IMediator _mediator;
        private static readonly string[] ValidRanges = { "day", "week", "month", "year" };
        private static readonly string[] ValidStatuses = { "pending", "verified", "closed", "malicious","solved" };
        private static readonly string[] ValidCitizenStatuses = { "pending", "verified", "closed", "malicious", "solved", "cancelled" };
        private readonly IWardRepository _wardRepo;

        public IncidentReportService(IIncidentReportRepository reportRepo, INoteRepository noteRepo, IFirebaseStorageService storageService, IAccountRepository accountRepo, IAchievementRepository achievementRepo, IConfiguration configuration, IDistrictRepository districtRepo, IMediator mediator, IWardRepository wardRepo)
        {
            _reportRepo = reportRepo;
            _noteRepo = noteRepo;
            _storageService = storageService;
            _accountRepo = accountRepo;
            _achievementRepo = achievementRepo;
            _configuration = configuration;
            _districtRepo = districtRepo;
            _mediator = mediator;
            _wardRepo = wardRepo;
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

            int? districtId = null;
            if (model.Address.ToLower().Contains("hồ chí minh"))
            {
                var districtName = ExtractDistrictName(model.Address);
                if (!string.IsNullOrEmpty(districtName))
                {
                    var district = await _districtRepo.GetByNameAsync(districtName);
                    if (district != null)
                    {
                        districtId = district.Id;
                    }
                }
            }
            int? wardId = null;
            if (!string.IsNullOrEmpty(model.Address) && districtId != null)
            {
                var wardName = ExtractWardName(model.Address);
                if (!string.IsNullOrEmpty(wardName))
                {
                    var ward = await _wardRepo.GetByNameAndDistrictAsync(wardName, districtId.Value);
                    if (ward != null)
                    {
                        wardId = ward.Id;
                    }
                }
            }


            if (!Enum.IsDefined(typeof(IncidentType), model.Type))
            {
                var validTypes = IncidentTypeHelper.GetAllDisplayValues()
                    .Select(t => t.DisplayName)
                    .ToList();

                throw new InvalidOperationException($"Loại sự cố không hợp lệ. Các loại hợp lệ gồm: {string.Join(", ", validTypes)}");
            }




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
                CreatedAt = DateTime.UtcNow,
                ImageUrls = uploadedImageUrls.Any() ? System.Text.Json.JsonSerializer.Serialize(uploadedImageUrls) : null,
                VideoUrl = uploadedVideoUrl,
                DistrictId = districtId,
                WardId = wardId,
            };

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
                    int rewardPoint = _configuration.GetValue<int>("Reward:VerifiedReportPoint", 10);
                    int newTotal = account.TotalPoint + rewardPoint;
                    await _mediator.Publish(new PointChangedEvent
                    {
                        UserId = account.Id,
                        NewTotalPoint = newTotal
                    });

                }
            }
            if (model.Status == "malicious")
            {
                var reporter = await _accountRepo.GetByIdAsync(report.UserId);
                if (reporter != null)
                {
                    reporter.ReputationPoint = Math.Max(0, reporter.ReputationPoint - 1);
                    await _accountRepo.UpdateOfficerAsync(reporter);
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
                Type = IncidentTypeHelper.GetAllDisplayValues()
                .FirstOrDefault(t => t.Value == report.Type.ToString()).DisplayName ?? "Không xác định",
                Description = report.Description,
                Lat = report.Lat,
                Lng = report.Lng,
                Address = report.Address,
                Status = report.Status,
                IsAnonymous = report.IsAnonymous,
                CreatedAt = report.CreatedAt,
                VerifiedByName = report.Verifier?.FullName,
                StatusMessage = report.StatusMessage,
                UserName = report.IsAnonymous ? null : report.User.FullName,
                DistrictName = report.District?.Name,
                WardName = report.Ward?.Name,
                Notes = report.Notes.Select(n => $"[{n.CreatedAt:yyyy-MM-dd HH:mm}] {n.Officer.FullName}: {n.Content}").ToList(),
                ImageUrls = string.IsNullOrEmpty(report.ImageUrls)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(report.ImageUrls),
                VideoUrl = report.VideoUrl,

            };
        }

        public async Task<ReportResponseModel> CancelAsync(Guid reportId, Guid userId, string? reason = null)
        {
            var report = await _reportRepo.GetByIdAsync(reportId);
            if (report == null || report.UserId != userId)
                throw new KeyNotFoundException("Không tìm thấy report hoặc bạn không có quyền huỷ.");

            if (report.Status != "pending")
                throw new InvalidOperationException("Chỉ có thể huỷ report khi trạng thái là 'pending'.");

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

            if (officer.DistrictId == null)
                throw new InvalidOperationException("Bạn chưa được gán khu vực quản lý.");


            var allReports = await _reportRepo.GetAllAsync();
            var filtered = allReports
                .Where(r => r.DistrictId == officer.DistrictId)
                .Select(ToResponseModel);

            return filtered;
        }

        public async Task<IEnumerable<GroupedReportResponseModel>> GetFilteredReportsByOfficerAsync(Guid officerId, string? range, string? status, string? wardName = null, bool includeRelated = false)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null)
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");

            if (officer.Role?.Name?.ToLower() != "officer")
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập chức năng này.");

            if (officer.DistrictId == null)
                throw new InvalidOperationException("Bạn chưa được gán khu vực quản lý.");

            var allReports = (await _reportRepo.GetAllAsync())
                .Where(r => r.DistrictId == officer.DistrictId)
                .ToList();

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
            if (!string.IsNullOrWhiteSpace(wardName))
            {
                if (!officer.DistrictId.HasValue)
                    throw new InvalidOperationException("Tài khoản chưa được gán khu vực.");
                var ward = await _wardRepo.GetByNameAndDistrictAsync(wardName.Trim(), officer.DistrictId.Value);
                if (ward == null)
                    throw new InvalidOperationException("Phường không hợp lệ trong khu vực bạn phụ trách.");

                allReports = allReports.Where(r => r.WardId == ward.Id).ToList();
            }


            var results = new List<GroupedReportResponseModel>();
            var visited = new HashSet<Guid>();

            allReports = allReports.OrderByDescending(r => r.CreatedAt).ToList();
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


        public async Task<IEnumerable<CitizenReportResponseModel>> GetFilteredReportsByCitizenAsync(Guid citizenId, string? range, string? status)
        {
            var reports = (await _reportRepo.GetAllAsync())
                .Where(r => r.UserId == citizenId);

            if (!string.IsNullOrEmpty(status))
            {
                if (!ValidCitizenStatuses.Contains(status.ToLower()))
                    throw new ArgumentException($"Giá trị 'status' không hợp lệ. Hợp lệ: {string.Join(", ", ValidCitizenStatuses)}");

                reports = reports.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
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

                reports = reports.Where(r => r.CreatedAt >= fromDate);
            }

            return reports.OrderByDescending(r => r.CreatedAt).Select(r => new CitizenReportResponseModel
            {
                Id = r.Id,
                Type = IncidentTypeHelper.GetAllDisplayValues()
                .FirstOrDefault(t => t.Value == r.Type.ToString()).DisplayName ?? "Không xác định",
                Description = r.Description,
                Address = r.Address,
                Status = r.Status,
                StatusMessage = r.StatusMessage,
                IsAnonymous = r.IsAnonymous,
                CreatedAt = r.CreatedAt,
                ImageUrls = string.IsNullOrEmpty(r.ImageUrls)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(r.ImageUrls),
                VideoUrl = r.VideoUrl
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

            var newDistrict = await _districtRepo.GetByIdAsync(model.NewDistrictId);
            if (newDistrict == null)
                throw new InvalidOperationException("Khu vực mới không hợp lệ.");

            if (report.DistrictId == model.NewDistrictId)
                throw new InvalidOperationException("Báo cáo đã thuộc khu vực này.");

            report.DistrictId = model.NewDistrictId;

            report.VerifiedBy = officerId;

            await _reportRepo.UpdateAsync(report);

            if (!string.IsNullOrWhiteSpace(model.Note))
            {
                var note = new Note
                {
                    OfficerId = officerId,
                    ReportId = reportId,
                    Content = model.Note,
                    CreatedAt = DateTime.UtcNow
                };
                await _noteRepo.CreateAsync(note);
            }

            var updated = await _reportRepo.GetByIdAsync(reportId);
            return ToResponseModel(updated!);
        }






    }

}
