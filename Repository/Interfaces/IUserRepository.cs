using BusinessObject.Models;

namespace Repository.Interfaces;

public interface IUserRepository
{
    Task<Account> CreateAsync(Account account);
    Task<Account?> GetByEmailAsync(string email);
    Task UpdateAsync(Account account);
    Task<Account> GetByRefreshTokenAsync(string refreshToken);
    List<Account> GetAll();

    Account GetAccountById(Guid id);
    Task<Account?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);

    void AddAccount(Account account);
}