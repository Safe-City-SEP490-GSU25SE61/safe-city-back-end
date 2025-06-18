using BusinessObject.Models;
using Service.Interfaces;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.DTOs.RequestModels;

namespace Service
{
    public class WardService : IWardService
    {
        private readonly IWardRepository _wardRepository;

        public WardService(IWardRepository wardRepository)
        {
            _wardRepository = wardRepository;
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
                DistrictId = w.DistrictId
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
                DistrictId = ward.DistrictId
            };
        }

        public async Task<int> CreateAsync(CreateWardDTO createWardDTO)
        {
            var ward = new Ward
            {
                Name = createWardDTO.Name,
                TotalReportedIncidents = createWardDTO.TotalReportedIncidents,
                DangerLevel = createWardDTO.DangerLevel,
                Note = createWardDTO.Note,
                PolygonData = createWardDTO.PolygonData,
                DistrictId = createWardDTO.DistrictId
            };

            await _wardRepository.CreateAsync(ward);

            return ward.Id;
        }

        public async Task UpdateAsync(int id, CreateWardDTO wardDTO)
        {
           
            var ward = await _wardRepository.GetByIdAsync(id);
            if (ward == null)
                throw new KeyNotFoundException("Ward not found.");

            ward.Name = wardDTO.Name;
            ward.TotalReportedIncidents = wardDTO.TotalReportedIncidents;
            ward.DangerLevel = wardDTO.DangerLevel;
            ward.Note = wardDTO.Note;
            ward.PolygonData = wardDTO.PolygonData;

            await _wardRepository.UpdateAsync(ward);
        }


        public async Task DeleteAsync(int id)
        {
            await _wardRepository.DeleteAsync(id);
        }

        public async Task<WardDTO> GetByNameAsync(string name)
        {
            var ward = await _wardRepository.GetByNameAsync(name);
            if (ward == null) return null;

            return new WardDTO
            {
                Id = ward.Id,
                Name = ward.Name,
                TotalReportedIncidents = ward.TotalReportedIncidents,
                DangerLevel = ward.DangerLevel,
                Note = ward.Note,
                PolygonData = ward.PolygonData,
                DistrictId = ward.DistrictId
            };
        }
    }
}
