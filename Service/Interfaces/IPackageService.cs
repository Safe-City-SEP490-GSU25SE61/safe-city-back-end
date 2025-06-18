using BusinessObject.DTOs.RequestModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPackageService
    {
        Task<IEnumerable<Package>> GetAllPackagesAsync();
        Task<Package> GetPackageByIdAsync(int packageId);
        Task CreatePackageAsync(CreatePackageDTO dto);
        Task UpdatePackageAsync(int packageId, UpdatePackageDTO dto);
        Task DeletePackageAsync(int packageId);
    }
}
