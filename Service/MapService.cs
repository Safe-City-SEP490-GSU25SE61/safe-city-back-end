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

        private static readonly string[] ValidStatuses = { "verified", "solved" };
        private static readonly string[] ValidRanges = { "hour", "day", "week" };

        public MapService(IIncidentReportRepository reportRepo, ICommuneRepository communeRepo)
        {
            _reportRepo = reportRepo;
            _communeRepo = communeRepo;
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

        public async Task<MapReportResponse> GetReportsForMapAsync(int communeId)
        {
            var allReports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    ValidStatuses.Contains(r.Status.Trim().ToLower()))
                .ToList();

            var from = DateTime.UtcNow.AddDays(-7);
            var reports = allReports
                .Where(r => r.OccurredAt >= from)
                .ToList();


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


        public async Task<IEnumerable<MapReportDetailDTO>> GetReportDetailsForMapAsync(int communeId, string? type, string? range)
        {
            var reports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    ValidStatuses.Contains(r.Status.Trim().ToLower()))
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

            DateTime from = DateTime.UtcNow.AddHours(-1); 
            if (!string.IsNullOrWhiteSpace(range))
            {
                var normalized = range.ToLower();
                if (!ValidRanges.Contains(normalized))
                    throw new ArgumentException("Giá trị 'range' không hợp lệ. Hợp lệ: hour, day, week");

                from = normalized switch
                {
                    "hour" => DateTime.UtcNow.AddHours(-1),
                    "day" => DateTime.UtcNow.AddDays(-1),
                    "week" => DateTime.UtcNow.AddDays(-7),
                    _ => DateTime.UtcNow.AddHours(-1)
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
                        r.OccurredAt >= from);
                await _communeRepo.UpdateAsync(commune);
            }

            return reports.OrderByDescending(r => r.OccurredAt).Select(report => new MapReportDetailDTO
            {
                Id = report.Id,
                CommuneId = report.CommuneId,
                Type = report.Type.ToString(),
                SubCategory = report.Type switch
                {
                    IncidentType.Traffic => report.TrafficSubCategory?.ToString(),
                    IncidentType.Security => report.SecuritySubCategory?.ToString(),
                    IncidentType.Infrastructure => report.InfrastructureSubCategory?.ToString(),
                    IncidentType.Environment => report.EnvironmentSubCategory?.ToString(),
                    IncidentType.Other => report.OtherSubCategory?.ToString(),
                    _ => null
                },
                Description = report.Description,
                Address = report.Address,
                Lat = report.Lat,
                Lng = report.Lng,
                OccurredAt = DateTimeHelper.ToVietnamTime(report.OccurredAt),
                Status = report.Status
            }).ToList();
        }


    }

}
