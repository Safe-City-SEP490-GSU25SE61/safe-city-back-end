using BusinessObject.DTOs;
using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ICommuneService
    {
        Task<IEnumerable<DistrictDTO>> GetAllAsync();
        Task<DistrictDTO> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateDistrictDTO createDistrictDTO);
        Task UpdateAsync(int id, CreateDistrictDTO districtDTO);
        Task DeleteAsync(int id);
        Task<IEnumerable<DistrictDTO>> SearchAsync(string? name, int? totalReportedIncidents);
        Task<bool> AssignDistrictToOfficerAsync(Guid accountId, int districtId);
        Task<IEnumerable<GroupedAssignOfficerChangeDTO>> GetHistoryByAccountIdAsync(Guid accountId);
        Task<bool> UnassignDistrictFromOfficerAsync(Guid accountId);
        Task<IEnumerable<CommuneForCitizenDTO>> GetAllForCitizenAsync(int provinceId);
        Task<object> GetStatisticsAsync();

    }

}
