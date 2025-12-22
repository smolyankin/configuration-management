using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.NotificationSubscriptions.Dto;
using ConfigurationManagement.Domain.Enums;

namespace ConfigurationManagement.Application.NotificationSubscriptions.Commands.Subscribe;

/// <summary>
/// Команда подписки на уведомления о событиях конфигурации.
/// </summary>
public record SubscribeCommand : Command<NotificationSubscriptionDto>
{
    /// <summary>
    /// Типы событий конфигурации для подписки (пустой список означает подписку на все события).
    /// </summary>
    public List<ConfigurationEventType> ConfigurationEventTypes { get; init; } = new();
}