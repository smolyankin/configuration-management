using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.Configurations.Dto;

namespace ConfigurationManagement.Application.Configurations.Commands.CreateConfiguration;

/// <summary>
/// Команда создания новой конфигурации.
/// </summary>
public record CreateConfigurationCommand : Command<ConfigurationDto>
{
    public string Name { get; init; } = string.Empty;
    public string Data { get; init; } = string.Empty;
}