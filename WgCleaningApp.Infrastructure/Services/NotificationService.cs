using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using WgCleaningApp.Domain.Entities;
using WgCleaningApp.Infrastructure.Persistence;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WgCleaningApp.Infrastructure.Services;

public class NotificationService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public NotificationService(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task SendNotificationAsync(Guid userId, string message)
    {
        // 1. Save in-app notification
        var notification = new AppNotification
        {
            UserId = userId,
            Message = message
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // 2. Send push notification if device token exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user?.DeviceToken != null)
        {
            await SendPushAsync(user.DeviceToken, message);
        }
    }

    public async Task SendNotificationToWgAsync(Guid wgId, string message)
    {
        var users = await _context.Users
            .Where(u => u.WgId == wgId)
            .ToListAsync();

        foreach (var user in users)
        {
            // Save in-app notification
            var notification = new AppNotification
            {
                UserId = user.Id,
                Message = message
            };

            _context.Notifications.Add(notification);

            // Send push if token exists
            if (!string.IsNullOrWhiteSpace(user.DeviceToken))
            {
                await SendPushAsync(user.DeviceToken, message);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SendPushAsync(string expoToken, string message)
    {
        var payload = new
        {
            to = expoToken,
            sound = "default",
            title = "WG Cleaning",
            body = message,
            priority = "high"
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://exp.host/--/api/v2/push/send",
            payload
        );

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Expo push error: {error}");
        }
    }
}
