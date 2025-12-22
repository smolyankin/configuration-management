using ConfigurationManagement.Domain.Entities;

namespace ConfigurationManagement.Application.Configurations.Dto;

/// <summary>
/// Конфигурации с пагинацией.
/// </summary>
public record ConfigurationsDto
{
    public List<ConfigurationDto> Configurations { get; init; } = new();
    public PaginationInfo Pagination { get; set; } = new();

}
