using BusinessObject.DTOs.ResponseModels;
using BusinessObject.Models;
using DataAccessLayer.DataContext;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CurrentSubscriptionResponseModel?> GetCurrentSubscriptionAsync(Account user)
        {
            if (user.Subscriptions == null)
                return new CurrentSubscriptionResponseModel
                {
                    PackageName = "No Subcription",
                    RemainingTime = "0d 0h 0m"
                };

            var subscription = await _context.Subscriptions
                .Where(s => s.UserId == user.Id && s.IsActive && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();


            if (subscription == null)
                return new CurrentSubscriptionResponseModel
                {
                    PackageName = "No Subcription",
                    RemainingTime = "0d 0h 0m"
                };

            var now = DateTime.UtcNow;
            var endDateTime = subscription.EndDate;

            if (endDateTime <= now)
            {
                return new CurrentSubscriptionResponseModel
                {
                    PackageName = subscription.Package.Name,
                    RemainingTime = "0d 0h 0m"
                };
            }

            var remaining = endDateTime - now;

            string remainingTime = $"{remaining.Value.Days}d {remaining.Value.Hours}h {remaining.Value.Minutes}m";

            return new CurrentSubscriptionResponseModel
            {
                PackageName = subscription.Package?.Name ?? "Unknown",
                RemainingTime = remainingTime
            };
        }

        public async Task<Subscription?> GetActiveByUserIdAsync(Guid userId)
        {
            return await _context.Subscriptions
               .Include(s => s.Package)
               .Where(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow)
               .OrderByDescending(s => s.EndDate)
               .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.Package)
                .Include(s => s.Account)
                .Include(s => s.Payment)
                .ToListAsync();
        }

    }
}
