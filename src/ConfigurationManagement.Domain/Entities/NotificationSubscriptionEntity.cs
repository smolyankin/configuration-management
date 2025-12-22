using System.ComponentModel.DataAnnotations.Schema;
using ConfigurationManagement.Domain.Enums;

namespace ConfigurationManagement.Domain.Entities;

/// <summary>
/// Подписка пользователя на конфигурации.
/// </summary>
[Table("notification_subscriptions")]
public class NotificationSubscriptionEntity : BaseEntity
{
    /// <summary>
    /// Идентификатор пользователя (подписчик).
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Список типов событий конфигураций (пустой список - подписан на все типы событий).
    /// </summary>
    public ICollection<ConfigurationEventType> ConfigurationEventTypes { get; set; } = [];

    /// <summary>
    /// Пользователь.
    /// </summary>
    public UserEntity? User { get; set; }
}