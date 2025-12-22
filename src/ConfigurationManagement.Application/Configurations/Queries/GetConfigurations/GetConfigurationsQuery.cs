using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.Configurations.Dto;

namespace ConfigurationManagement.Application.Configurations.Queries.GetConfigurations;

/// <summary>
/// Получить конфигурации пользователя с пагинацией и фильтрацией.
/// </summary>
public record GetConfigurationsQuery : Query<ConfigurationsDto>
{
    public string? Name { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
