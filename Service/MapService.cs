using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Enums;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MapService : IMapService
    {
        private readonly IIncidentReportRepository _reportRepo;
        private readonly ICommuneRepository _communeRepo;
        private readonly IAccountRepository _accountRepo;

        private static readonly string[] ValidStatuses = { "verified", "solved" };
        private static readonly string[] ValidRanges = { "week", "month", "quarter" };
        private static readonly string[] OfficerStatuses = { "pending", "verified", "solved" };


        public MapService(IIncidentReportRepository reportRepo, ICommuneRepository communeRepo, IAccountRepository accountRepo)
        {
            _reportRepo = reportRepo;
            _communeRepo = communeRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IEnumerable<MapCommuneDTO>> GetAllCommunePolygonsAsync()
        {
            var communes = await _communeRepo.GetAllAsync();
            return communes
                .Where(c => c.IsActive && !string.IsNullOrWhiteSpace(c.PolygonData))
                .Select(c => new MapCommuneDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Polygon = c.PolygonData 
                });
        }

        public async Task<MapReportResponse> GetReportsForMapAsync(int communeId, string? type, string? range)
        {

            DateTime from = DateTime.UtcNow.AddDays(-7); 
            if (!string.IsNullOrWhiteSpace(range))
            {
                var normalized = range.ToLower();
                if (!ValidRanges.Contains(normalized))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: week, month, quarter");

                from = normalized switch
                {
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "quarter" => DateTime.UtcNow.AddMonths(-3),
                    _ => DateTime.UtcNow.AddDays(-7) 
                };
            }

            var rawReports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    ValidStatuses.Contains(r.Status.Trim().ToLower()) &&
                    r.IsVisibleOnMap &&
                    r.OccurredAt >= from)
                .ToList();

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<IncidentType>(type, true, out var parsedType))
                    throw new ArgumentException("Loại sự cố không hợp lệ.");
                rawReports = rawReports.Where(r => r.Type == parsedType).ToList();
            }

            var reports = rawReports;

            var reportsByType = reports
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => IncidentTypeHelper.GetAllDisplayValues()
                        .FirstOrDefault(x => x.Value == g.Key.ToString()).DisplayName ?? g.Key.ToString(),
                    g => g.Count());

            var totalReportsSameCommune = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    ValidStatuses.Contains(r.Status.Trim().ToLower()) &&
                    r.OccurredAt >= from)
                .Count();

            var commune = await _communeRepo.GetByIdAsync(communeId);
            var reportsByCommune = new Dictionary<string, int>
            {
                [commune?.Name ?? "Không rõ"] = rawReports.Count(r => r.CommuneId == communeId && r.IsVisibleOnMap)
            };


            return new MapReportResponse
            {
                ReportsByType = reportsByType,
                ReportsByCommune = reportsByCommune
            };
        }




        public async Task<IEnumerable<MapReportDetailDTO>> GetReportDetailsForMapAsync(int communeId, string? type, string? range)
        {
            var reports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    ValidStatuses.Contains(r.Status.Trim().ToLower()) &&
                    r.IsVisibleOnMap)
                .ToList();

            IncidentType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<IncidentType>(type, true, out var incidentType))
                {
                    var validTypes = IncidentTypeHelper.GetAllDisplayValues()
                        .Select(t => t.DisplayName)
                        .ToList();
                    throw new ArgumentException($"Loại sự cố không hợp lệ. Các loại hợp lệ gồm: {string.Join(", ", validTypes)}");
                }

                parsedType = incidentType;
                reports = reports.Where(r => r.Type == incidentType).ToList();
            }

            DateTime from = DateTime.UtcNow.AddDays(-7); 
            if (!string.IsNullOrWhiteSpace(range))
            {
                var normalized = range.ToLower();
                if (!ValidRanges.Contains(normalized))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: week, month, quarter");

                from = normalized switch
                {
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "quarter" => DateTime.UtcNow.AddMonths(-3),
                    _ => DateTime.UtcNow.AddDays(-7) 
                };
            }

            reports = reports.Where(r => r.OccurredAt >= from).ToList();

            var commune = await _communeRepo.GetByIdAsync(communeId);
            if (commune != null)
            {
                commune.TotalReportedIncidents = (await _reportRepo.GetAllAsync())
                    .Count(r =>
                        r.CommuneId == communeId &&
                        !string.IsNullOrWhiteSpace(r.Status) &&
                        ValidStatuses.Contains(r.Status.Trim().ToLower()) &&
                        r.IsVisibleOnMap &&
                        r.OccurredAt >= from);
                await _communeRepo.UpdateAsync(commune);
            }

            return reports.OrderByDescending(r => r.OccurredAt).Select(report => new MapReportDetailDTO
            {
                Id = report.Id,
                CommuneId = report.CommuneId,
                Type = IncidentTypeHelper.GetAllDisplayValues()
                .FirstOrDefault(t => t.Value == report.Type.ToString()).DisplayName ?? "Không xác định",
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
                },
                Address = report.Address,
                Lat = report.Lat,
                Lng = report.Lng,
                OccurredAt = DateTimeHelper.ToVietnamTime(report.OccurredAt),
                Status = report.Status
            }).ToList();
        }


        public async Task<MapReportResponse> GetOfficerReportsForMapAsync(Guid officerId, string? type, string? range)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null || officer.CommuneId == null)
                throw new InvalidOperationException("Không xác định được khu vực của cán bộ.");

            var communeId = officer.CommuneId.Value;
            DateTime from = DateTime.UtcNow.AddDays(-7); 

            if (!string.IsNullOrWhiteSpace(range))
            {
                var normalized = range.ToLower();
                if (!ValidRanges.Contains(normalized))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: week, month, quarter");

                from = normalized switch
                {
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "quarter" => DateTime.UtcNow.AddMonths(-3),
                    _ => DateTime.UtcNow.AddDays(-7) 
                };
            }

            var rawReports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    OfficerStatuses.Contains(r.Status.Trim().ToLower()) &&
                    r.OccurredAt >= from)
                .ToList();

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<IncidentType>(type, true, out var parsedType))
                    throw new ArgumentException("Loại sự cố không hợp lệ.");
                rawReports = rawReports.Where(r => r.Type == parsedType).ToList();
            }

            var reports = rawReports;

            var reportsByType = reports
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => IncidentTypeHelper.GetAllDisplayValues()
                        .FirstOrDefault(x => x.Value == g.Key.ToString()).DisplayName ?? g.Key.ToString(),
                    g => g.Count());

            var commune = await _communeRepo.GetByIdAsync(communeId);
            var reportsByCommune = new Dictionary<string, int>
            {
                [commune?.Name ?? "Không rõ"] = reports.Count
            };

            return new MapReportResponse
            {
                ReportsByType = reportsByType,
                ReportsByCommune = reportsByCommune
            };
        }




        public async Task<IEnumerable<MapReportDetailDTO>> GetOfficerReportDetailsForMapAsync(Guid officerId, string? type, string? range)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null || officer.CommuneId == null)
                throw new InvalidOperationException("Không tìm thấy khu vực của cán bộ.");

            var communeId = officer.CommuneId.Value;

            var reports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    OfficerStatuses.Contains(r.Status.Trim().ToLower()))
                .ToList();

            IncidentType? parsedType = null;
            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<IncidentType>(type, true, out var incidentType))
                {
                    var validTypes = IncidentTypeHelper.GetAllDisplayValues()
                        .Select(t => t.DisplayName);
                    throw new ArgumentException($"Loại sự cố không hợp lệ. Các loại gồm: {string.Join(", ", validTypes)}");
                }
                parsedType = incidentType;
                reports = reports.Where(r => r.Type == incidentType).ToList();
            }

            DateTime from = DateTime.UtcNow.AddDays(-7);
            if (!string.IsNullOrWhiteSpace(range))
            {
                var normalized = range.ToLower();
                if (!ValidRanges.Contains(normalized))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: week, month, quarter");

                from = normalized switch
                {
                    "week" => DateTime.UtcNow.AddDays(-7),
                    "month" => DateTime.UtcNow.AddMonths(-1),
                    "quarter" => DateTime.UtcNow.AddMonths(-3),
                    _ => DateTime.UtcNow.AddDays(-7) 
                };
            }

            reports = reports.Where(r => r.OccurredAt >= from).ToList();

            return reports
                .OrderByDescending(r => r.OccurredAt)
                .Select(report => new MapReportDetailDTO
                {
                    Id = report.Id,
                    CommuneId = report.CommuneId,
                    Type = IncidentTypeHelper.GetAllDisplayValues()
                    .FirstOrDefault(t => t.Value == report.Type.ToString()).DisplayName ?? "Không xác định",
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
                    },
                    Address = report.Address,
                    Lat = report.Lat,
                    Lng = report.Lng,
                    OccurredAt = DateTimeHelper.ToVietnamTime(report.OccurredAt),
                    Status = report.Status
                })
                .ToList();
        }



    }

}
