namespace ConfigurationManagement.Application.Configurations.Dto;

/// <summary>
/// Версии конфигурации с пагинацией.
/// </summary>
public class ConfigurationVersionsDto
{
    public List<ConfigurationVersionDto> Versions { get; set; } = new();
    public PaginationInfo Pagination { get; set; } = new();
}
