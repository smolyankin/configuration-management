using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Enums;
using ConfigurationManagement.Domain.Exceptions;
using ConfigurationManagement.Infrastructure.Hubs;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Infrastructure.Services;

/// <summary>
/// Сервис отправки уведомлений.
/// </summary>
public class NotifyService : INotifyService
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly ILogger<NotifyService> _logger;

    public NotifyService(
        ConfigurationDbContext dbContext,
        IHubContext<NotificationsHub> hubContext,
        ILogger<NotifyService> logger)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Send(Guid configurationId, ConfigurationEventType configurationEventType, CancellationToken cancellationToken)
    {
        var configuration = await _dbContext.Configurations
            .Include(c => c.Versions)
            .FirstOrDefaultAsync(c => c.Id == configurationId, CancellationToken.None)
            ?? throw new NotFoundException($"Configuration with ID '{configurationId}' not found.");

        var subscribers = await _dbContext.NotificationSubscriptions
            .AsNoTracking()
            .Where(s => s.ConfigurationEventTypes.Contains(configurationEventType)
                || s.ConfigurationEventTypes.Count == 0)
            .ToListAsync(CancellationToken.None);

        if (subscribers.Count == 0)
            return;

        var userIds = subscribers.DistinctBy(s => s.UserId)
            .Select(s => s.UserId.ToString())
            .ToArray();

        try
        {
            await _hubContext.Clients.Users(userIds)
                .SendAsync("ReceiveConfigurationNotification", configuration, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error sending notification to userIds {userIds}", userIds);
        }

        _logger.LogInformation("Sent {EventType} notification for configuration {ConfigurationId} to {Count} subscribers",
            configurationEventType, configuration.Id, subscribers.Count);
    }
}