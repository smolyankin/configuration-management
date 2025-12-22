using ConfigurationManagement.Domain.Enums;

namespace ConfigurationManagement.Domain.Abstractions;

/// <summary>
/// Сервис отправки уведомлений.
/// </summary>
public interface INotifyService
{
    /// <summary>
    /// Отправить уведомление.
    /// </summary>
    Task Send(Guid ConfigurationId, ConfigurationEventType ConfigurationEventType, CancellationToken cancellationToken);
}