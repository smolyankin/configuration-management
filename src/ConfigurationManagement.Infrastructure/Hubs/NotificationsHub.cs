using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Infrastructure.Hubs;

/// <summary>
/// Хаб для уведомлений.
/// </summary>
[Authorize]
public class NotificationsHub : Hub
{
    private readonly ILogger<NotificationsHub> _logger;

    public NotificationsHub(ILogger<NotificationsHub> logger) => _logger = logger;

    public override async Task OnConnectedAsync()
    {
        try
        {
            // Логируем ВСЮ информацию для диагностики
            _logger.LogInformation("""
                OnConnectedAsync called. 
                ConnectionId: {ConnectionId}
                User authenticated: {IsAuthenticated}
                User claims count: {ClaimsCount}
                User identifier: {UserIdentifier}
                Headers: {Headers}
                """,
                Context.ConnectionId,
                Context.User?.Identity?.IsAuthenticated,
                Context.User?.Claims.Count(),
                Context.UserIdentifier,
                string.Join(", ", Context.GetHttpContext()?.Request.Headers.Select(h => $"{h.Key}: {h.Value}")));

            var connectionId = Context.ConnectionId;
            var user = Context.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("Connection {ConnectionId} rejected: User not authenticated", connectionId);
                Context.Abort();
                return;
            }

            try
            {
                var userId = GetUserId();
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
                _logger.LogInformation("User {UserId} connected", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering connection for user with connection {ConnectionId}", connectionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in OnConnectedAsync for Connection {ConnectionId}",
                Context.ConnectionId);
            Context.Abort();
            throw;
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var user = Context.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            _logger.LogWarning("Connection {ConnectionId} rejected: User not authenticated", connectionId);
            Context.Abort();
            return;
        }

        var userId = GetUserId();
        try
        {
            _logger.LogInformation("User {UserId} disconnected", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering connection for user {UserId}", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Valid user ID not found in token claims.");
        }

        return userId;
    }
}