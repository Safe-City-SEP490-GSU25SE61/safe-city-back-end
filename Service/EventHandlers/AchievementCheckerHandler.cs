using BusinessObject.Events;
using MediatR;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EventHandlers
{
    public class AchievementCheckerHandler : INotificationHandler<PointChangedEvent>
    {
        private readonly IAchievementRepository _achievementRepo;
        private readonly IAccountRepository _accountRepo;

        public AchievementCheckerHandler(IAchievementRepository achievementRepo, IAccountRepository accountRepo)
        {
            _achievementRepo = achievementRepo;
            _accountRepo = accountRepo;
        }

        public async Task Handle(PointChangedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[MEDIATR] Đã nhận PointChangedEvent cho user {notification.UserId} với điểm {notification.NewTotalPoint}");
            var account = await _accountRepo.GetByIdAsync(notification.UserId);
            if (account == null) return;
            account.TotalPoint = notification.NewTotalPoint;

            var achievements = await _achievementRepo.GetAllAsync();
            var matched = achievements
                .Where(a => a.MinPoint <= notification.NewTotalPoint)
                .OrderByDescending(a => a.MinPoint)
                .FirstOrDefault();

            
            if (account.AchievementId != matched?.Id)
            {
                Console.WriteLine($"[MEDIATR] Cập nhật danh hiệu từ {account.AchievementId} -> {matched?.Id}");
                account.AchievementId = matched?.Id; 
               
            }
            await _accountRepo.UpdateOfficerAsync(account);
        }

    }
}
