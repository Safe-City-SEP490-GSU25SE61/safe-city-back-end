using BusinessObject.Models;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs;
using Repository.Repositories;

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
                PolygonData = d.PolygonData
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
                PolygonData = district.PolygonData
            };
        }

        public async Task<int> CreateAsync(CreateDistrictDTO createDistrictDTO)
        {
            var district = new District
            {
                Name = createDistrictDTO.Name,
                TotalReportedIncidents = createDistrictDTO.TotalReportedIncidents,
                DangerLevel = createDistrictDTO.DangerLevel,
                Note = createDistrictDTO.Note,
                PolygonData = createDistrictDTO.PolygonData
            };

            await _districtRepository.CreateAsync(district);

            return district.Id;
        }

        public async Task UpdateAsync(int id, CreateDistrictDTO districtDTO)
        {
            var district = await _districtRepository.GetByIdAsync(id);
            if (district == null)
                throw new KeyNotFoundException("District not found.");

            district.Name = districtDTO.Name;
            district.TotalReportedIncidents = districtDTO.TotalReportedIncidents;
            district.DangerLevel = districtDTO.DangerLevel;
            district.Note = districtDTO.Note;
            district.PolygonData = districtDTO.PolygonData;

            await _districtRepository.UpdateAsync(district);
        }

        public async Task DeleteAsync(int id)
        {
            await _districtRepository.DeleteAsync(id);
        }

        public async Task<DistrictDTO> GetByNameAsync(string name)
        {
            var district = await _districtRepository.GetByNameAsync(name);
            if (district == null) return null;

            return new DistrictDTO
            {
                Id = district.Id,
                Name = district.Name,
                TotalReportedIncidents = district.TotalReportedIncidents,
                DangerLevel = district.DangerLevel,
                Note = district.Note,
                PolygonData = district.PolygonData
            };
        }
        public async Task<bool> AssignDistrictToOfficerAsync(Guid accountId, int districtId)
        {
            
            var district = await _districtRepository.GetByIdAsync(districtId);
            if (district == null)
            {
                throw new KeyNotFoundException("District not found.");
            }

            
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null || account.RoleId != 2) 
            {
                throw new InvalidOperationException("Account not found or not an officer.");
            }

           
            account.DistrictId = districtId;
            await _accountRepository.UpdateAsync(account);

            return true; 
        }
    }
}
