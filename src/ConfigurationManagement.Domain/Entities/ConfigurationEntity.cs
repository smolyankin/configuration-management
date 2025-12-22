using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConfigurationManagement.Domain.Entities;

/// <summary>
/// Конфигурация пользователя.
/// </summary>
[Table("configurations")]
public class ConfigurationEntity : BaseEntity
{
    /// <summary>
    /// Наименование (уникально для пользователя).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ID пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Конфигурация в JSON.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Пользователь.
    /// </summary>
    [JsonIgnore]
    public UserEntity User { get; set; } = null!;

    /// <summary>
    /// Версии конфигурации.
    /// </summary>
    public ICollection<ConfigurationVersionEntity> Versions { get; set; } = [];
}