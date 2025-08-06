using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ReputationResetCronJob : BackgroundService
{
    private readonly ILogger<ReputationResetCronJob> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ReputationResetCronJob(
        ILogger<ReputationResetCronJob> logger,
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
                var now = DateTime.UtcNow;

                if(now.Day == 1 && now.Hour == 0)
                {
                    _logger.LogInformation("ReputationResetCronJob: Đang tăng uy tín...");

                    using var scope = _scopeFactory.CreateScope();
                    var accountRepo = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                    var accounts = await accountRepo.GetAllAsync();

                    foreach (var acc in accounts)
                    {
                        if (acc.ReputationPoint < 3)
                        {
                            acc.ReputationPoint += 1;
                            await accountRepo.UpdateOfficerAsync(acc);
                            _logger.LogInformation($"User {acc.FullName}: tăng uy tín lên {acc.ReputationPoint}");
                        }
                    }

                    _logger.LogInformation("ReputationResetCronJob: Hoàn tất.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy ReputationResetCronJob.");
            }
        }
    }
}
