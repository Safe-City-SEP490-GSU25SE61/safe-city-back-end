using BusinessObject.Models;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs.RequestModels;
using Repository;

namespace Service
{
    public class WardService : IWardService
    {
        private readonly IWardRepository _wardRepository;
        private readonly IDistrictRepository _districtRepository;

        public WardService(IWardRepository wardRepository, IDistrictRepository districtRepository)
        {
            _wardRepository = wardRepository;
            _districtRepository = districtRepository;
        }

        public async Task<IEnumerable<WardDTO>> GetAllAsync()
        {
            var wards = await _wardRepository.GetAllAsync();
            return wards.Select(w => new WardDTO
            {
                Id = w.Id,
                Name = w.Name,
                TotalReportedIncidents = w.TotalReportedIncidents,
                DangerLevel = w.DangerLevel,
                Note = w.Note,
                PolygonData = w.PolygonData,
                DistrictId = w.DistrictId,
                CreateAt = w.CreateAt,
                LastUpdated = w.LastUpdated,
                IsActive = w.IsActive
            }).ToList();
        }

        public async Task<WardDTO> GetByIdAsync(int id)
        {
            var ward = await _wardRepository.GetByIdAsync(id);
            if (ward == null) return null;

            return new WardDTO
            {
                Id = ward.Id,
                Name = ward.Name,
                TotalReportedIncidents = ward.TotalReportedIncidents,
                DangerLevel = ward.DangerLevel,
                Note = ward.Note,
                PolygonData = ward.PolygonData,
                DistrictId = ward.DistrictId,
                CreateAt = ward.CreateAt,
                LastUpdated = ward.LastUpdated,
                IsActive = ward.IsActive
            };
        }

        public async Task<int> CreateAsync(CreateWardDTO createWardDTO)
        {
            var district = await _districtRepository.GetByIdAsync(createWardDTO.DistrictId);
            if (district == null || !district.IsActive)
            {
                throw new KeyNotFoundException("Phường không hợp lệ hoặc đã bị xóa.");
            }
            var ward = new Ward
            {
                Name = createWardDTO.Name,
                TotalReportedIncidents = 0,
                DangerLevel = 0,
                Note = createWardDTO.Note,
                PolygonData = createWardDTO.PolygonData,
                DistrictId = createWardDTO.DistrictId,
                CreateAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsActive = true
            };

            await _wardRepository.CreateAsync(ward);

            return ward.Id;
        }

        public async Task UpdateAsync(int id, CreateWardDTO wardDTO)
        {
           
            var ward = await _wardRepository.GetByIdAsync(id);
            if (ward == null)
                throw new KeyNotFoundException("Phường không tìm thấy.");
            if (!ward.IsActive)
                throw new InvalidOperationException("Phường đã xóa, không thể cập nhật.");
            var district = await _districtRepository.GetByIdAsync(wardDTO.DistrictId);
            if (district == null || !district.IsActive)
            {
                throw new KeyNotFoundException("Quận không hợp lệ hoặc đã bị xóa.");
            }


            ward.Name = wardDTO.Name;
            ward.TotalReportedIncidents = 0;
            ward.DangerLevel = 0;
            ward.Note = wardDTO.Note;
            ward.PolygonData = wardDTO.PolygonData;
            ward.LastUpdated = DateTime.UtcNow;

            await _wardRepository.UpdateAsync(ward);
        }


        public async Task DeleteAsync(int id)
        {
            var ward = await _wardRepository.GetByIdAsync(id);
            if (ward == null)
                throw new KeyNotFoundException("Phường không tìm thấy.");
            if (!ward.IsActive)
                throw new InvalidOperationException("Phường đã xóa trước.");
            
            ward.IsActive = false;
            await _wardRepository.UpdateAsync(ward);
        }

        public async Task<IEnumerable<WardDTO>> SearchAsync(string? name, int? totalReportedIncidents, int? dangerLevel, string? districtName)
        {
            var wards = await _wardRepository.SearchAsync(name, totalReportedIncidents, dangerLevel, districtName);

            return wards.Select(w => new WardDTO
            {
                Id = w.Id,
                Name = w.Name,
                TotalReportedIncidents = w.TotalReportedIncidents,
                DangerLevel = w.DangerLevel,
                Note = w.Note,
                PolygonData = w.PolygonData,
                DistrictId = w.DistrictId
            }).ToList();
        }
    }
}
