using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountResponseModel>> GetAllAsync();
        Task<AccountResponseModel> GetByIdAsync(Guid id);
        Task<AccountResponseModel> AddAsync(AddAccountRequestModel requestModel);
        Task<AccountResponseModel> UpdateAsync(Guid id, UpdateAccountRequestModel requestModel);
        Task<AccountResponseModel> DeleteAsync(Guid id);
    }
}
