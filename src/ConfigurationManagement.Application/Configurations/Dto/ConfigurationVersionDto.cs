namespace ConfigurationManagement.Application.Configurations.Dto;

/// <summary>
/// Версия конфигурации.
/// </summary>
public class ConfigurationVersionDto
{
    public Guid Id { get; set; }
    public Guid ConfigurationId { get; set; }
    public int VersionNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public object Data { get; set; } = new();
    public string DataJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}