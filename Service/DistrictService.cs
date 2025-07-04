using BusinessObject.Models;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs;
using Repository.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Repository;

namespace Service
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IWardRepository _wardRepository;
        private readonly IAssignOfficerHistoryRepository _assignOfficerHistoryRepository;

        public DistrictService(IDistrictRepository districtRepository, IAccountRepository accountRepository, IWardRepository wardRepository, IAssignOfficerHistoryRepository assignOfficerHistoryRepository)
        {
            _districtRepository = districtRepository;
            _accountRepository = accountRepository;
            _wardRepository = wardRepository;
            _assignOfficerHistoryRepository = assignOfficerHistoryRepository;
        }


        public async Task<IEnumerable<DistrictDTO>> GetAllAsync()
        {
            var districts = await _districtRepository.GetAllAsync();
            var allWards = await _wardRepository.GetAllAsync();
            var allAccounts = await _accountRepository.GetAllAsync();
            return districts.Select(d => new DistrictDTO
            {
                Id = d.Id,
                Name = d.Name,
                TotalReportedIncidents = d.TotalReportedIncidents,
                DangerLevel = d.DangerLevel,
                Note = d.Note,
                PolygonData = d.PolygonData,
                CreateAt = d.CreateAt,
                LastUpdated = d.LastUpdated,
                IsActive = d.IsActive,
                WardNames = allWards
                    .Where(w => w.DistrictId == d.Id && w.IsActive)
                    .Select(w => w.Name)
                    .ToList(),  
                TotalAssignedOfficers = allAccounts
            .Count(a => a.DistrictId == d.Id && a.RoleId == 3 && a.Status == "active")
            }).ToList();
        }

        public async Task<DistrictDTO> GetByIdAsync(int id)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null) return null;

            var allWards = await _wardRepository.GetAllAsync();
            var allAccounts = await _accountRepository.GetAllAsync();

            var wardNames = allWards
                .Where(w => w.DistrictId == district.Id && w.IsActive)
                .Select(w => w.Name)
                .ToList();

            var totalOfficers = allAccounts
                .Count(a => a.DistrictId == district.Id && a.RoleId == 3 && a.Status == "active");

            return new DistrictDTO
            {
                Id = district.Id,
                Name = district.Name,
                TotalReportedIncidents = district.TotalReportedIncidents,
                DangerLevel = district.DangerLevel,
                Note = district.Note,
                PolygonData = district.PolygonData,
                CreateAt = district.CreateAt,
                LastUpdated = district.LastUpdated,
                IsActive = district.IsActive,

                WardNames = wardNames,
                TotalAssignedOfficers = totalOfficers
            };
        }


        public async Task<int> CreateAsync(CreateDistrictDTO createDistrictDTO)
        {
            var district = new District
            {
                Name = createDistrictDTO.Name,
                TotalReportedIncidents = 0,  
                DangerLevel = 0,  
                Note = createDistrictDTO.Note,
                PolygonData = createDistrictDTO.PolygonData,
                CreateAt = DateTime.UtcNow,  
                LastUpdated = DateTime.UtcNow,  
                IsActive = true  
            };

            await _districtRepository.CreateAsync(district);

            return district.Id;
        }

        public async Task UpdateAsync(int id, CreateDistrictDTO districtDTO)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null)
                throw new KeyNotFoundException("Quận không tồn tại");
            if (!district.IsActive)
                throw new InvalidOperationException("Quận không thể cập nhật khi đã vô hiệu hóa.");
            
            district.Name = districtDTO.Name;
            district.TotalReportedIncidents = 0;  
            district.DangerLevel = 0;  
            district.Note = districtDTO.Note;
            district.PolygonData = districtDTO.PolygonData;
            district.LastUpdated = DateTime.UtcNow;

            await _districtRepository.UpdateAsync(district);
        }

        public async Task DeleteAsync(int id)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null)
                throw new KeyNotFoundException("Quận không tồn tại");
            if (!district.IsActive)
                throw new InvalidOperationException("Quận đã xóa trước đó.");

            district.IsActive = false;  
            await _districtRepository.UpdateAsync(district);
        }


        public async Task<bool> AssignDistrictToOfficerAsync(Guid accountId, int districtId)
        {
            
            var district = await _districtRepository.GetByIdAsync(districtId);
            if (district == null || !district.IsActive)
            {
                throw new KeyNotFoundException("Quận không tìm thấy hoặc đã bị vô hiệu hóa.");
            }

            
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null || account.RoleId != 3) 
            {
                throw new InvalidOperationException("Tài khoản không tìm thấy hoặc không phải là officer.");
            }

            var oldDistrictId = account.DistrictId;


            if (oldDistrictId != districtId)
            {
                var changedAt = DateTime.UtcNow;

                var log = new AssignOfficerHistory
                {
                    AccountId = account.Id,
                    OldDistrictId = oldDistrictId,
                    NewDistrictId = districtId,
                    ChangedAt = changedAt
                };

                await _assignOfficerHistoryRepository.CreateAsync(log);
            }

            account.DistrictId = districtId;
            await _accountRepository.UpdateOfficerAsync(account);

            return true; 
        }
        public async Task<IEnumerable<DistrictDTO>> SearchAsync(string? name, int? totalReportedIncidents, int? dangerLevel)
        {
            var districts = await _districtRepository.SearchAsync(name, totalReportedIncidents, dangerLevel);

            return districts.Select(d => new DistrictDTO
            {
                Id = d.Id,
                Name = d.Name,
                TotalReportedIncidents = d.TotalReportedIncidents,
                DangerLevel = d.DangerLevel,
                Note = d.Note,
                PolygonData = d.PolygonData
            }).ToList();
        }
        public async Task<IEnumerable<GroupedAssignOfficerChangeDTO>> GetHistoryByAccountIdAsync(Guid accountId)
        {
            var history = await _assignOfficerHistoryRepository.GetByAccountIdAsync(accountId);
            var allDistricts = await _districtRepository.GetAllAsync();

            string GetDistrictName(int? id) =>
            id == null ? "Chưa được phân công" : allDistricts.FirstOrDefault(d => d.Id == id)?.Name ?? "Không xác định";


            return history
                .GroupBy(h => h.ChangedAt)
                .Select(g => new GroupedAssignOfficerChangeDTO
                {
                    ChangedAt = g.Key,
                    Changes = g.Select(h => new AssignOfficerChangeDTO
                    {
                        OldDistrictName = GetDistrictName(h.OldDistrictId),
                        NewDistrictName = GetDistrictName(h.NewDistrictId)
                    }).ToList()
                })
                .OrderByDescending(x => x.ChangedAt)
                .ToList();
        }

        public async Task<bool> UnassignDistrictFromOfficerAsync(Guid accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null || account.RoleId != 3)
                throw new InvalidOperationException("Tài khoản không tồn tại hoặc không phải là officer.");

            if (account.DistrictId == null)
                return false;

            var log = new AssignOfficerHistory
            {
                AccountId = account.Id,
                OldDistrictId = account.DistrictId,
                NewDistrictId = null, 
                ChangedAt = DateTime.UtcNow
            };

            account.DistrictId = null;
            await _accountRepository.UpdateOfficerAsync(account);
            await _assignOfficerHistoryRepository.CreateAsync(log);

            return true;
        }


    }
}
