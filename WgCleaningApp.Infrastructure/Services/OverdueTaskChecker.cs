using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WgCleaningApp.Infrastructure.Persistence;
namespace WgCleaningApp.Infrastructure.Services;

public class OverdueTaskChecker : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<OverdueTaskChecker> _logger;

    public OverdueTaskChecker(IServiceProvider provider, ILogger<OverdueTaskChecker> logger)
    {
        _provider = provider;
        _logger = logger;

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
        try
        {
            using var scope = _provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var notifications = scope.ServiceProvider.GetRequiredService<NotificationService>();

            // 1) CLEANUP OLD NOTIFICATIONS (older than 30 days)
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var oldNotifications = context.Notifications
                .Where(n => n.CreatedAt < cutoffDate);

            context.Notifications.RemoveRange(oldNotifications);
            await context.SaveChangesAsync(stoppingToken);

            // OVERDUE TASKS CHECKER (tasks that are not completed and started more than 7 days ago)
            var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));

            var overdueTasks = await context.Tasks
                .Where(t => !t.IsCompleted && t.StartDate <= cutoff)
                .ToListAsync(stoppingToken);

            // FOR REAL PUSH NOTIFCATIONS
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

        catch (Exception ex) {

            _logger.LogError(ex, "Error while checking overdue tasks.");

        }
        
    }

}
