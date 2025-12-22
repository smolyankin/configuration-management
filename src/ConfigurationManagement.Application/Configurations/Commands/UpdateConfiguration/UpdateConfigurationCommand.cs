using ConfigurationManagement.Application.Common.Base;
using ConfigurationManagement.Application.Configurations.Dto;

namespace ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;

/// <summary>
/// Команда обновления существующей конфигурации (автоматически создает новую версию).
/// </summary>
public record UpdateConfigurationCommand : Command<ConfigurationDto>
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Data { get; init; }
}