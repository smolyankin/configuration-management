using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.Configurations.Dto;

namespace ConfigurationManagement.Application.Configurations.Commands.RestoreConfigurationVersion;

/// <summary>
/// Команда восстановления конфигурации до указанной версии.
/// </summary>
public record RestoreConfigurationVersionCommand : Command<ConfigurationDto>
{
    public Guid ConfigurationId { get; init; }
    public int VersionNumber { get; init; }
}