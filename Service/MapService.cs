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
using System.Text.Json;

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
                var fromForUpdate = DateTime.UtcNow.AddMonths(-3);
                commune.TotalReportedIncidents = (await _reportRepo.GetAllAsync())
                    .Count(r =>
                        r.CommuneId == communeId &&
                        !string.IsNullOrWhiteSpace(r.Status) &&
                        ValidStatuses.Contains(r.Status.Trim().ToLower()) &&
                        r.IsVisibleOnMap &&
                        r.OccurredAt >= fromForUpdate);
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

        public async Task<MapReportDetailsPolygonDTO> GetOfficerReportDetailsWithPolygonAsync(Guid officerId, string? type, string? range)
        {
            var officer = await _accountRepo.GetByIdAsync(officerId);
            if (officer == null || officer.CommuneId == null)
                throw new InvalidOperationException("Không tìm thấy khu vực của cán bộ.");

            var reports = await GetOfficerReportDetailsForMapAsync(officerId, type, range); 
            var commune = await _communeRepo.GetByIdAsync(officer.CommuneId.Value);

            return new MapReportDetailsPolygonDTO
            {
                Polygon = commune?.PolygonData,
                Reports = reports
            };
        }

        public async Task<MapReportResponse> GetAdminReportsForMapAsync(string? type, string? range)
        {
            DateTime from = DateTime.UtcNow.AddDays(-7);
            if (!string.IsNullOrWhiteSpace(range))
            {
                var normalized = range.Trim().ToLowerInvariant();
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

            var allReports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    OfficerStatuses.Contains(r.Status.Trim().ToLower()) &&
                    r.OccurredAt >= from)
                .ToList();

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<IncidentType>(type, true, out var parsedType))
                    throw new ArgumentException("Loại sự cố không hợp lệ.");
                allReports = allReports.Where(r => r.Type == parsedType).ToList();
            }

            var reportsByType = allReports
                .GroupBy(r => r.Type)
                .ToDictionary(
                    g => IncidentTypeHelper.GetAllDisplayValues()
                            .FirstOrDefault(x => x.Value == g.Key.ToString()).DisplayName ?? g.Key.ToString(),
                    g => g.Count()
                );

            var communes = await _communeRepo.GetAllAsync();
            var nameById = communes.ToDictionary(c => c.Id, c => c.Name);

            var reportsByCommune = allReports
                .Where(r => r.CommuneId.HasValue)                  
                .GroupBy(r => r.CommuneId!.Value)                  
                .Select(g => new
                {
                    Name = nameById.TryGetValue(g.Key, out var nm) ? nm : $"Commune {g.Key}",
                    Count = g.Count()
                })
                .OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToDictionary(x => x.Name, x => x.Count);          

            return new MapReportResponse
            {
                ReportsByType = reportsByType,
                ReportsByCommune = reportsByCommune
            };
        }




        public async Task<MapReportDetailsDTO> GetAdminReportDetailsAdminAsync(int communeId, string? type, string? range)
        {
            var reports = (await _reportRepo.GetAllAsync())
                .Where(r =>
                    r.CommuneId == communeId &&
                    !string.IsNullOrWhiteSpace(r.Status) &&
                    OfficerStatuses.Contains(r.Status.Trim().ToLower()))
                .ToList();

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!Enum.TryParse<IncidentType>(type, true, out var incidentType))
                {
                    var validTypes = IncidentTypeHelper.GetAllDisplayValues().Select(t => t.DisplayName);
                    throw new ArgumentException($"Loại sự cố không hợp lệ. Các loại gồm: {string.Join(", ", validTypes)}");
                }
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

            var mapped = reports
                .OrderByDescending(r => r.OccurredAt)
                .Select(report => new MapReportDetailDTO
                {
                    Id = report.Id,
                    CommuneId = report.CommuneId,
                    Type = IncidentTypeHelper.GetAllDisplayValues()
                              .FirstOrDefault(t => t.Value == report.Type.ToString()).DisplayName ?? "Không xác định",
                    SubCategory = report.Type switch
                    {
                        IncidentType.Traffic => IncidentTypeHelper.GetDisplayValues<TrafficSubCategory>()
                                                       .FirstOrDefault(x => x.Value == report.TrafficSubCategory?.ToString()).DisplayName,
                        IncidentType.Security => IncidentTypeHelper.GetDisplayValues<SecuritySubCategory>()
                                                       .FirstOrDefault(x => x.Value == report.SecuritySubCategory?.ToString()).DisplayName,
                        IncidentType.Infrastructure => IncidentTypeHelper.GetDisplayValues<InfrastructureSubCategory>()
                                                       .FirstOrDefault(x => x.Value == report.InfrastructureSubCategory?.ToString()).DisplayName,
                        IncidentType.Environment => IncidentTypeHelper.GetDisplayValues<EnvironmentSubCategory>()
                                                       .FirstOrDefault(x => x.Value == report.EnvironmentSubCategory?.ToString()).DisplayName,
                        IncidentType.Other => IncidentTypeHelper.GetDisplayValues<OtherSubCategory>()
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

            var commune = await _communeRepo.GetByIdAsync(communeId);
            var pt = ExtractPointFromPolygon(commune?.PolygonData);
            var pointDto = pt.HasValue ? new MapPointDTO { Lat = (decimal)pt.Value.lat, Lng = (decimal)pt.Value.lng } : null;


            return new MapReportDetailsDTO
            {
                Point = pointDto,
                Reports = mapped
            };
        }



        private static (double lat, double lng)? ExtractPointFromPolygon(string? polygonJson)
        {
            if (string.IsNullOrWhiteSpace(polygonJson)) return null;

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(polygonJson);
                var root = doc.RootElement;

                static (double lat, double lng)? ReadPointFromGeometry(System.Text.Json.JsonElement geom)
                {
                    if (geom.ValueKind != System.Text.Json.JsonValueKind.Object) return null;
                    if (!geom.TryGetProperty("type", out var t) || t.GetString() is not string type) return null;
                    if (!geom.TryGetProperty("coordinates", out var coords)) return null;

                    if (string.Equals(type, "Point", StringComparison.OrdinalIgnoreCase) &&
                        coords.ValueKind == System.Text.Json.JsonValueKind.Array &&
                        coords.GetArrayLength() >= 2 &&
                        coords[0].TryGetDouble(out var lng) &&
                        coords[1].TryGetDouble(out var lat))
                    {
                        return (lat, lng); 
                    }
                    return null;
                }

                if (root.TryGetProperty("type", out var t1) &&
                    string.Equals(t1.GetString(), "FeatureCollection", StringComparison.OrdinalIgnoreCase) &&
                    root.TryGetProperty("features", out var feats) &&
                    feats.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var f in feats.EnumerateArray())
                    {
                        if (f.TryGetProperty("geometry", out var g))
                        {
                            var pt = ReadPointFromGeometry(g);
                            if (pt.HasValue) return pt;
                        }
                    }
                }


                if (root.TryGetProperty("type", out var t2) &&
                    string.Equals(t2.GetString(), "Feature", StringComparison.OrdinalIgnoreCase) &&
                    root.TryGetProperty("geometry", out var g2))
                {
                    var pt = ReadPointFromGeometry(g2);
                    if (pt.HasValue) return pt;
                }


                System.Text.Json.JsonElement coordsEl;
                if ((root.TryGetProperty("geometry", out var geom) && geom.TryGetProperty("coordinates", out coordsEl)) ||
                    root.TryGetProperty("coordinates", out coordsEl))
                {

                }
                else if (root.TryGetProperty("features", out var feats2) && feats2.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var f in feats2.EnumerateArray())
                    {
                        if (f.TryGetProperty("geometry", out var g) && g.TryGetProperty("coordinates", out coordsEl))
                        { goto HAVE_COORDS; }
                    }
                    return null;
                }
                else
                {
                    return null;
                }

            HAVE_COORDS:
                
                var ring = coordsEl;
                while (ring.ValueKind == System.Text.Json.JsonValueKind.Array &&
                       ring.GetArrayLength() > 0 &&
                       ring[0].ValueKind == System.Text.Json.JsonValueKind.Array &&
                       ring[0].GetArrayLength() > 0 &&
                       ring[0][0].ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    ring = ring[0]; 
                }

                if (ring.ValueKind == System.Text.Json.JsonValueKind.Array &&
                    ring.GetArrayLength() > 0 &&
                    ring[0].ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    double sumLat = 0, sumLng = 0; int n = 0;
                    foreach (var pos in ring.EnumerateArray())
                    {
                        if (pos.ValueKind == System.Text.Json.JsonValueKind.Array &&
                            pos.GetArrayLength() >= 2 &&
                            pos[0].TryGetDouble(out var lng) &&
                            pos[1].TryGetDouble(out var lat))
                        {
                            sumLat += lat; sumLng += lng; n++;
                        }
                    }
                    if (n > 0) return (sumLat / n, sumLng / n);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }


    }

}
