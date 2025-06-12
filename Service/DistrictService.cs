using BusinessObject.Models;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.DTOs;

namespace Service
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;

        public DistrictService(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
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

        public async Task CreateAsync(DistrictDTO districtDTO)
        {
            var district = new District
            {
                Name = districtDTO.Name,
                TotalReportedIncidents = districtDTO.TotalReportedIncidents,
                DangerLevel = districtDTO.DangerLevel,
                Note = districtDTO.Note,
                PolygonData = districtDTO.PolygonData
            };

            await _districtRepository.CreateAsync(district);
        }

        public async Task UpdateAsync(DistrictDTO districtDTO)
        {
            var district = await _districtRepository.GetByIdAsync(districtDTO.Id);
            if (district == null) return;

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
    }
}
