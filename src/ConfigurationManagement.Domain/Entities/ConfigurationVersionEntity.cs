using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConfigurationManagement.Domain.Entities;

/// <summary>
/// Версия конфигурации.
/// </summary>
[Table("configuration_versions")]
public class ConfigurationVersionEntity : BaseEntity
{
    /// <summary>
    /// Идентификатор конфигурации.
    /// </summary>
    public Guid ConfigurationId { get; set; }

    /// <summary>
    /// Номер версии (начинается с 1)
    /// </summary>
    public int VersionNumber { get; set; }

    /// <summary>
    /// Наименование конфигурации.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Конфигурация в JSON.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Конфигурация.
    /// </summary>
    [JsonIgnore]
    public ConfigurationEntity Configuration { get; set; } = null!;
}