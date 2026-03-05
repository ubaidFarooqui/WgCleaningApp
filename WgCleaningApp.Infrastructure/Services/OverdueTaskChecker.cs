using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WgCleaningApp.Infrastructure.Persistence;
namespace WgCleaningApp.Infrastructure.Services;

public class OverdueTaskChecker : BackgroundService
{
    private readonly IServiceProvider _provider;

    public OverdueTaskChecker(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait 5 seconds to allow the app to fully start
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        // Run immediately on startup if delay above is deleted
        await CheckOverdueTasks(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1); // midnight UTC
            var delay = nextRun - now;

            await Task.Delay(delay, stoppingToken);

            await CheckOverdueTasks(stoppingToken);
        }
    }

    private async Task CheckOverdueTasks(CancellationToken stoppingToken)
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notifications = scope.ServiceProvider.GetRequiredService<NotificationService>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var overdueTasks = await context.Tasks
            .Where(t => !t.IsCompleted)
            .Where(t => t.StartDate.AddDays(7) <= today)
            .ToListAsync(stoppingToken);

        // For real push notification 
        foreach (var task in overdueTasks)
        {
            if (task.AssignedToUserId != null)
            {
                await notifications.SendNotificationAsync(
                    task.AssignedToUserId.Value,
                    $"Hey you missed your cleaning task: {task.Title}"
                );
            }
        }
    }

}
