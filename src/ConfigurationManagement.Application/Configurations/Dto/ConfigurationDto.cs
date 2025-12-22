namespace ConfigurationManagement.Application.Configurations.Dto;

/// <summary>
/// Конфигурация.
/// </summary>
public class ConfigurationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Data { get; set; } = string.Empty;
}