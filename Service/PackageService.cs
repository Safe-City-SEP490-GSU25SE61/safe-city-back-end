using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository;
using Repository.Interfaces;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IPackageChangeHistoryRepository _changeHistoryRepository;

        public PackageService(IPackageRepository packageRepository, IPackageChangeHistoryRepository changeHistoryRepository)
        {
            _packageRepository = packageRepository;
            _changeHistoryRepository = changeHistoryRepository;
        }

        public async Task<IEnumerable<PackageDTO>> GetAllPackagesAsync()
        {
            var packages = await _packageRepository.GetAllAsync();
            return packages.Select(p => new PackageDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DurationDays = p.DurationDays,
                Color = p.Color,
                CreateAt = DateTimeHelper.ToVietnamTime(p.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(p.LastUpdated),
                MonthlyVirtualEscortLimit = p.MonthlyVirtualEscortLimit,
                CanViewIncidentDetail = p.CanViewIncidentDetail,
                CanReusePreviousEscortPaths = p.CanReusePreviousEscortPaths,
                CanPostBlog = p.CanPostBlog,
                IsActive = p.IsActive
            }).ToList();
        }


        public async Task<PackageDTO> GetPackageByIdAsync(int packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Package not found.");
            }

            return new PackageDTO
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                DurationDays = package.DurationDays,
                Color = package.Color,
                CreateAt = DateTimeHelper.ToVietnamTime(package.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(package.LastUpdated),
                MonthlyVirtualEscortLimit = package.MonthlyVirtualEscortLimit,
                CanViewIncidentDetail = package.CanViewIncidentDetail,
                CanReusePreviousEscortPaths = package.CanReusePreviousEscortPaths,
                CanPostBlog = package.CanPostBlog,
                IsActive = package.IsActive
            };
        }


        public async Task<int> CreatePackageAsync(CreatePackageDTO dto)
        {
            var package = new Package
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
                Color = dto.Color,
                CreateAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                CanPostBlog = dto.CanPostBlog,
                CanReusePreviousEscortPaths = dto.CanReusePreviousEscortPaths,  
                CanViewIncidentDetail = dto.CanViewIncidentDetail,
                MonthlyVirtualEscortLimit = dto.MonthlyVirtualEscortLimit,
                IsActive = true
            };

            await _packageRepository.CreateAsync(package);

            return package.Id;  
        }


        public async Task UpdatePackageAsync(int packageId, UpdatePackageDTO dto)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
                throw new KeyNotFoundException("Gói không tìm thấy.");

            var changedAt = DateTime.UtcNow;
            var logs = new List<PackageChangeHistory>();

            if (package.Description != dto.Description)
            {
                logs.Add(new PackageChangeHistory
                {
                    PackageId = package.Id,
                    FieldName = "Description",
                    OldValue = package.Description,
                    NewValue = dto.Description,
                    ChangedAt = changedAt
                });
                package.Description = dto.Description;
            }

            if (package.Price != dto.Price)
            {
                logs.Add(new PackageChangeHistory
                {
                    PackageId = package.Id,
                    FieldName = "Price",
                    OldValue = package.Price.ToString(),
                    NewValue = dto.Price.ToString(),
                    ChangedAt = changedAt
                });
                package.Price = dto.Price;
            }

            if (package.DurationDays != dto.DurationDays)
            {
                logs.Add(new PackageChangeHistory
                {
                    PackageId = package.Id,
                    FieldName = "DurationDays",
                    OldValue = package.DurationDays.ToString(),
                    NewValue = dto.DurationDays.ToString(),
                    ChangedAt = changedAt
                });
                package.DurationDays = dto.DurationDays;
            }

            package.Color = dto.Color;
            package.LastUpdated = changedAt;
            package.CanPostBlog = dto.CanPostBlog;
            package.CanReusePreviousEscortPaths = dto.CanReusePreviousEscortPaths;
            package.CanViewIncidentDetail = dto.CanViewIncidentDetail;
            package.MonthlyVirtualEscortLimit = dto.MonthlyVirtualEscortLimit;

            if (logs.Any())
                await _changeHistoryRepository.AddManyAsync(logs);

            await _packageRepository.UpdateAsync(package);
        }


        public async Task DeletePackageAsync(int packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Gói không tìm thấy.");
            }

            
            package.IsActive = false;

            await _packageRepository.UpdateAsync(package);
        }
        public async Task<IEnumerable<GroupedPackageChangeDTO>> GetHistoryByIdAsync(int packageId)
        {
            var history = await _changeHistoryRepository.GetByPackageIdAsync(packageId);

            var grouped = history
                .GroupBy(h => h.ChangedAt)
                .OrderBy(g => g.Key)
                .ToList();

            var result = new List<GroupedPackageChangeDTO>();

            for (int i = 0; i < grouped.Count; i++)
            {
                var current = grouped[i];
                var next = i < grouped.Count - 1 ? grouped[i + 1].Key : (DateTime?)null;

                int? durationDays = null;
                var durationChange = current.FirstOrDefault(c => c.FieldName == "DurationDays");
                if (durationChange != null && int.TryParse(durationChange.NewValue, out var parsed))
                    durationDays = parsed;

                DateTime? expirationUtc = null;
                if (durationDays.HasValue)
                    expirationUtc = SafeAddDays(current.Key, durationDays.Value); // <- KHÔNG overflow

                result.Add(new GroupedPackageChangeDTO
                {
                    ChangedAt = DateTimeHelper.ToVietnamTime(current.Key),
                    EffectiveStart = DateTimeHelper.ToVietnamTime(current.Key),
                    EffectiveEnd = next.HasValue ? DateTimeHelper.ToVietnamTime(next.Value) : null,
                    PackageExpiration = expirationUtc != null ? DateTimeHelper.ToVietnamTime(expirationUtc.Value) : null,
                    Changes = current.Select(c => new PackageChangeDetailDTO
                    {
                        FieldName = c.FieldName,
                        FieldDisplayName = GetDisplayNameFromField<Package>(c.FieldName),
                        OldValue = c.OldValue,
                        NewValue = c.NewValue
                    }).ToList()
                });
            }

            return result.OrderByDescending(r => r.ChangedAt);
        }




        private string GetDisplayNameFromField<T>(string fieldName)
        {
            var prop = typeof(T).GetProperty(fieldName);
            var displayAttr = prop?.GetCustomAttributes(typeof(DisplayAttribute), true)
                                  .FirstOrDefault() as DisplayAttribute;

            return displayAttr?.Name ?? fieldName;
        }


        private static DateTime? SafeAddDays(DateTime basis, int days)
        {

            if (days < 0) return null;


            var maxAdd = (int)Math.Floor((DateTime.MaxValue - basis).TotalDays);
            if (days > maxAdd) return null;

            return basis.AddDays(days);
        }

    }
}


