using BusinessObject.DTOs.RequestModels;
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

        public async Task<IEnumerable<Package>> GetAllPackagesAsync()
        {
            return await _packageRepository.GetAllAsync();
        }

        public async Task<Package> GetPackageByIdAsync(int packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Package not found.");
            }

            return package;
        }

        public async Task CreatePackageAsync(CreatePackageDTO dto)
        {
            var package = new Package
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
                IsActive = dto.IsActive
            };

            await _packageRepository.CreateAsync(package);
        }

        public async Task UpdatePackageAsync(int packageId, UpdatePackageDTO dto)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Package not found.");
            }

            package.Description = dto.Description;
            package.Price = dto.Price;
            package.DurationDays = dto.DurationDays;
            package.IsActive = dto.IsActive;

            await _packageRepository.UpdateAsync(package);
        }

        public async Task DeletePackageAsync(int packageId)
        {
            var package = await _packageRepository.GetByIdAsync(packageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Package not found.");
            }

            // Đánh dấu gói dịch vụ là không còn hoạt động thay vì xóa
            package.IsActive = false;

            await _packageRepository.UpdateAsync(package);
        }
    }
}


