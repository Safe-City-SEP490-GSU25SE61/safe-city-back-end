using BusinessObject.Models;

namespace Repository.Interfaces;

public interface IUserRepository
{
    Task<Account> CreateAsync(Account account);
    Task<Account?> GetByEmailAsync(string email);
    Task<Account?> GetByPhoneAsync(string phone);
    Task<Account?> GetByIdNumberAsync(string idNumber);
    Task UpdateAsync(Account account);
    Task<Account> GetByRefreshTokenAsync(string refreshToken);
    List<Account> GetAll();

    Account GetAccountById(Guid id);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetProfileByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);

    void AddAccount(Account account);
}