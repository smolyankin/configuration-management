using ConfigurationManagement.Domain.Enums;

namespace ConfigurationManagement.Application.NotificationSubscriptions.Dto;

/// <summary>
/// Подписка на уведомления.
/// </summary>
public class NotificationSubscriptionDto
{
    /// <summary>
    /// Идентификатор подписки.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор пользователя (подписчик).
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Типы событий, на которые подписан пользователь.
    /// </summary>
    public List<ConfigurationEventType> EventTypes { get; init; } = new();

    /// <summary>
    /// Дата и время создания подписки.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Дата и время обновления подписки.
    /// </summary>
    public DateTime UpdatedAt { get; init; }
}