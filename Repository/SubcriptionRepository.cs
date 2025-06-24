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
    public class SubcriptionRepository : ISubcriptionRepository
    {
        private readonly AppDbContext _context;

        public SubcriptionRepository(AppDbContext context)
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

            var subscription = user.Subscriptions.FirstOrDefault(s => s.IsActive);

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

            string remainingTime = $"{remaining.Days}d {remaining.Hours}h {remaining.Minutes}m";

            return new CurrentSubscriptionResponseModel
            {
                PackageName = subscription.Package?.Name ?? "Unknown",
                RemainingTime = remainingTime
            };
        }
    }
}

