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
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;

        public PackageService(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
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
                CreateAt = p.CreateAt,
                LastUpdated = p.LastUpdated,
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
                CreateAt = package.CreateAt,
                LastUpdated = package.LastUpdated,
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
                CreateAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
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

            package.Description = dto.Description;
            package.Price = dto.Price;
            package.DurationDays = dto.DurationDays;
            package.LastUpdated= DateTime.UtcNow;

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
    }
}


