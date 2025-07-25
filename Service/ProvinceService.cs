using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceService(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public async Task<IEnumerable<ProvinceResponseDTO>> GetAllAsync()
        {
            var provinces = await _provinceRepository.GetAllAsync();
            return provinces.Select(p => new ProvinceResponseDTO
            {
                id = p.Id,        
                Name = p.Name
            });
        }

        public async Task<Province> GetByIdAsync(int id)
        {
            return await _provinceRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(CreateProvinceDTO province)
        {
            var newProvince = new Province()
            { 
                Name = province.Name,
                Note = province.Note,
            };
            await _provinceRepository.CreateAsync(newProvince);
        }

        public async Task UpdateAsync(Province province)
        {
            var existing = await _provinceRepository.GetByIdAsync(province.Id);
            if (existing == null) throw new Exception("Non-existed Province!");

            await _provinceRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _provinceRepository.GetByIdAsync(id);
            if (existing == null) throw new Exception("Non-existed Province!");

            await _provinceRepository.DeleteAsync(id);
        }
    }

}
