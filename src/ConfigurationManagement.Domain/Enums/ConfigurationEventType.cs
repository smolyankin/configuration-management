namespace ConfigurationManagement.Domain.Enums;

/// <summary>
/// Тип события конфигурации.
/// </summary>
public enum ConfigurationEventType
{
    /// <summary>
    /// Создание.
    /// </summary>
    Created = 1,

    /// <summary>
    /// Изменение.
    /// </summary>
    Modified = 2,

    /// <summary>
    /// Восстановление версии.
    /// </summary>
    Restored = 3
}
