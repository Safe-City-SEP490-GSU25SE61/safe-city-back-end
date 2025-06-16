using BusinessObject.DTOs.RequestModels;
using BusinessObject.DTOs.ResponseModels;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<AccountResponseModel> AddAsync(AddAccountRequestModel requestModel)
        {
            var accounts = await _accountRepository.GetAllAsync();
            if (accounts.FirstOrDefault(x => x.Email.ToLower() == requestModel.Email.ToLower()) != null)
            {
                throw new InvalidOperationException("Account email must be unique");
            }
            requestModel.Password = BCrypt.Net.BCrypt.HashPassword(requestModel.Password);
            //var result = await _accountRepository.AddAsync(requestModel.ToAccount());
            //return result.ToAccountResponseModel();
            return null;
        }

        public async Task<AccountResponseModel> DeleteAsync(Guid id)
        {
            var result = await _accountRepository.DeleteAsync(id);
            //return result.ToAccountResponseModel();
            return null;
        }

        public async Task<IEnumerable<AccountResponseModel>> GetAllAsync()
        {
            var result = await _accountRepository.GetAllAsync();
            //return result.Select(x => x.ToAccountResponseModel());
            return null;
        }

        public async Task<AccountResponseModel> GetByIdAsync(Guid id)
        {
            var result = await _accountRepository.GetByIdAsync(id);
            if (result == null) throw new KeyNotFoundException();
            //return result.ToAccountResponseModel();
            return null;
        }

        public async Task<AccountResponseModel> UpdateAsync(Guid id, UpdateAccountRequestModel requestModel)
        {
            var accounts = await _accountRepository.GetAllAsync();
            if (accounts.FirstOrDefault(x => x.Email.ToLower() == requestModel.Email.ToLower()) != null)
            {
                throw new InvalidOperationException("Account email must be unique");
            }
            //var result = await _accountRepository.UpdateAsync(requestModel.ToAccount(id));
            //return result.ToAccountResponseModel();
            return null;
        }
    }
}
