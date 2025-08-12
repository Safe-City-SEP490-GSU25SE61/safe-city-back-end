using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetActiveByUserIdAsync(Guid userId);
        Task AddAsync(Subscription subscription);
        Task<CurrentSubscriptionResponseModel?> GetCurrentSubscriptionAsync(Account user);
        Task UpdateAsync(Subscription subscription);
        Task<IEnumerable<Subscription>> GetAllAsync();

    }

}
