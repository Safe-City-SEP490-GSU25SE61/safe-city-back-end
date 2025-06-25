using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
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
        Task<IEnumerable<PackageDTO>> GetAllPackagesAsync();  
        Task<PackageDTO> GetPackageByIdAsync(int packageId);
        Task<int> CreatePackageAsync(CreatePackageDTO dto); 
        Task UpdatePackageAsync(int packageId, UpdatePackageDTO dto); 
        Task DeletePackageAsync(int packageId);
        Task<IEnumerable<GroupedPackageChangeDTO>> GetHistoryByIdAsync(int packageId);

    }
}
