using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 
using Repository.Interfaces;

public class AchievementCronJob : BackgroundService
{
    private readonly ILogger<AchievementCronJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public AchievementCronJob(
        ILogger<AchievementCronJob> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken); 


                _logger.LogInformation("Cron job: Kiểm tra achievement...");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var accountRepo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                    var achievementRepo = scope.ServiceProvider.GetRequiredService<IAchievementRepository>();

                    var accounts = await accountRepo.GetAllAsync();
                    var achievements = await achievementRepo.GetAllAsync();

                    foreach (var acc in accounts)
                    {
                        var matched = achievements
                            .Where(a => a.MinPoint <= acc.TotalPoint)
                            .OrderByDescending(a => a.MinPoint)
                            .FirstOrDefault();

                        if (acc.AchievementId != matched?.Id)
                        {
                            _logger.LogInformation($" User {acc.FullName}: {acc.AchievementId} → {matched?.Id}");
                            acc.AchievementId = matched?.Id;
                            await accountRepo.UpdateOfficerAsync(acc);
                        }
                    }
                }

                _logger.LogInformation("Cron job hoàn tất.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy cron job.");
            }
        }
    }
}
