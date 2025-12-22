using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.Configurations.Dto;

namespace ConfigurationManagement.Application.Configurations.Queries.GetConfigurationById;

/// <summary>
/// Получить конфигурацию по идентификатору.
/// </summary>
public record GetConfigurationByIdQuery : Query<ConfigurationDto?>
{
    public required Guid Id { get; init; }
}