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
using Service.Helpers;

namespace Service
{
    public class CommuneService : ICommuneService
    {
        private readonly ICommuneRepository _communeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAssignOfficerHistoryRepository _assignOfficerHistoryRepository;

        public CommuneService(ICommuneRepository districtRepository, IAccountRepository accountRepository, IAssignOfficerHistoryRepository assignOfficerHistoryRepository)
        {
            _communeRepository = districtRepository;
            _accountRepository = accountRepository;
            _assignOfficerHistoryRepository = assignOfficerHistoryRepository;
        }


        public async Task<IEnumerable<DistrictDTO>> GetAllAsync()
        {
            var districts = await _communeRepository.GetAllAsync();
            var allAccounts = await _accountRepository.GetAllAsync();
            return districts.Select(d => new DistrictDTO
            {
                Id = d.Id,
                Name = d.Name,
                TotalReportedIncidents = d.TotalReportedIncidents,
                Note = d.Note,
                PolygonData = d.PolygonData,
                CreateAt = DateTimeHelper.ToVietnamTime(d.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(d.LastUpdated),
                IsActive = d.IsActive,
                TotalAssignedOfficers = allAccounts
            .Count(a => a.CommuneId == d.Id && a.RoleId == 3 && a.Status == "active")
            }).ToList();
        }

        public async Task<IEnumerable<CommuneForCitizenDTO>> GetAllForCitizenAsync(int provinceId)
        {
            return await _communeRepository.GetAllActiveByProvinceAsync(provinceId);
        }

        public async Task<DistrictDTO> GetByIdAsync(int id)
        {
            var district = await _communeRepository.GetByIdAsync(id);
            if (district == null) return null;

            var allAccounts = await _accountRepository.GetAllAsync();



            var totalOfficers = allAccounts
                .Count(a => a.CommuneId == district.Id && a.RoleId == 3 && a.Status == "active");

            return new DistrictDTO
            {
                Id = district.Id,
                Name = district.Name,
                TotalReportedIncidents = district.TotalReportedIncidents,
                Note = district.Note,
                PolygonData = district.PolygonData,
                CreateAt = DateTimeHelper.ToVietnamTime(district.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(district.LastUpdated),
                IsActive = district.IsActive,

                TotalAssignedOfficers = totalOfficers
            };
        }


        public async Task<int> CreateAsync(CreateDistrictDTO createDistrictDTO)
        {
            var district = new Commune
            {
                Name = createDistrictDTO.Name,
                TotalReportedIncidents = 0,   
                Note = createDistrictDTO.Note,
                PolygonData = createDistrictDTO.PolygonData,
                CreateAt = DateTime.UtcNow,  
                LastUpdated = DateTime.UtcNow,  
                IsActive = true  
            };

            await _communeRepository.CreateAsync(district);

            return district.Id;
        }

        public async Task UpdateAsync(int id, CreateDistrictDTO districtDTO)
        {
            var district = await _communeRepository.GetByIdAsync(id);
            if (district == null)
                throw new KeyNotFoundException("Quận không tồn tại");
            if (!district.IsActive)
                throw new InvalidOperationException("Quận không thể cập nhật khi đã vô hiệu hóa.");
            
            district.Name = districtDTO.Name;
            district.TotalReportedIncidents = 0;  
            district.Note = districtDTO.Note;
            district.PolygonData = districtDTO.PolygonData;
            district.LastUpdated = DateTime.UtcNow;

            await _communeRepository.UpdateAsync(district);
        }

        public async Task DeleteAsync(int id)
        {
            var district = await _communeRepository.GetByIdAsync(id);
            if (district == null)
                throw new KeyNotFoundException("Quận không tồn tại");
            if (!district.IsActive)
                throw new InvalidOperationException("Quận đã xóa trước đó.");

            district.IsActive = false;  
            await _communeRepository.UpdateAsync(district);
        }


        public async Task<bool> AssignDistrictToOfficerAsync(Guid accountId, int districtId)
        {
            
            var district = await _communeRepository.GetByIdAsync(districtId);
            if (district == null || !district.IsActive)
            {
                throw new KeyNotFoundException("Quận không tìm thấy hoặc đã bị vô hiệu hóa.");
            }

            
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null || account.RoleId != 3) 
            {
                throw new InvalidOperationException("Tài khoản không tìm thấy hoặc không phải là officer.");
            }

            var oldDistrictId = account.CommuneId;


            if (oldDistrictId != districtId)
            {
                var changedAt = DateTime.UtcNow;

                var log = new AssignOfficerHistory
                {
                    AccountId = account.Id,
                    OldCommuneId = oldDistrictId,
                    NewCommuneId = districtId,
                    ChangedAt = changedAt
                };

                await _assignOfficerHistoryRepository.CreateAsync(log);
            }

            account.CommuneId = districtId;
            await _accountRepository.UpdateOfficerAsync(account);

            return true; 
        }
        public async Task<IEnumerable<DistrictDTO>> SearchAsync(string? name, int? totalReportedIncidents)
        {
            var communes = await _communeRepository.SearchAsync(name, totalReportedIncidents);

            return communes.Select(d => new DistrictDTO
            {
                Id = d.Id,
                Name = d.Name,
                TotalReportedIncidents = d.TotalReportedIncidents,
                Note = d.Note,
                PolygonData = d.PolygonData,
                CreateAt = DateTimeHelper.ToVietnamTime(d.CreateAt),
                LastUpdated = DateTimeHelper.ToVietnamTime(d.LastUpdated)
            }).ToList();
        }
        public async Task<IEnumerable<GroupedAssignOfficerChangeDTO>> GetHistoryByAccountIdAsync(Guid accountId)
        {
            var history = await _assignOfficerHistoryRepository.GetByAccountIdAsync(accountId);
            var allCommunes = await _communeRepository.GetAllAsync();

            string GetDistrictName(int? id) =>
            id == null ? "Chưa được phân công" : allCommunes.FirstOrDefault(d => d.Id == id)?.Name ?? "Không xác định";


            return history
                .GroupBy(h => h.ChangedAt)
                .Select(g => new GroupedAssignOfficerChangeDTO
                {
                    ChangedAt = DateTimeHelper.ToVietnamTime(g.Key),
                    Changes = g.Select(h => new AssignOfficerChangeDTO
                    {
                        OldCommuneName = GetDistrictName(h.OldCommuneId),
                        NewCommuneName = GetDistrictName(h.NewCommuneId)
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

            if (account.CommuneId == null)
                return false;

            var log = new AssignOfficerHistory
            {
                AccountId = account.Id,
                OldCommuneId = account.CommuneId,
                NewCommuneId = null, 
                ChangedAt = DateTime.UtcNow
            };

            account.CommuneId = null;
            await _accountRepository.UpdateOfficerAsync(account);
            await _assignOfficerHistoryRepository.CreateAsync(log);

            return true;
        }


    }
}
