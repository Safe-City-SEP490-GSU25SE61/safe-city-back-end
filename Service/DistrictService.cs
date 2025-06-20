using BusinessObject.Models;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs;
using Repository.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Service
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly IAccountRepository _accountRepository;

        public DistrictService(IDistrictRepository districtRepository, IAccountRepository accountRepository)
        {
            _districtRepository = districtRepository;
            _accountRepository = accountRepository;
        }


        public async Task<IEnumerable<DistrictDTO>> GetAllAsync()
        {
            var districts = await _districtRepository.GetAllAsync();
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
                IsActive = d.IsActive
            }).ToList();
        }

        public async Task<DistrictDTO> GetByIdAsync(int id)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null) return null;

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
                IsActive = district.IsActive

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
            if (district == null)
            {
                throw new KeyNotFoundException("District not found.");
            }

            
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null || account.RoleId != 3) 
            {
                throw new InvalidOperationException("Account not found or not an officer.");
            }

           
            account.DistrictId = districtId;
            await _accountRepository.UpdateAsync(account);

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
    }
}
