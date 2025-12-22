using ConfigurationManagement.Application.Configurations.Commands.RestoreConfigurationVersion;
using FluentValidation;

namespace ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;

/// <summary>
/// Валидатор изменения существующей конфигурации.
/// </summary>
public class RestoreConfigurationVersionCommandValidator : AbstractValidator<RestoreConfigurationVersionCommand>
{
    public RestoreConfigurationVersionCommandValidator()
    {
        RuleFor(x => x.ConfigurationId)
            .NotEmpty()
            .WithMessage("ConfigurationId is required.");

        RuleFor(x => x.VersionNumber)
            .NotEmpty()
            .WithMessage("VersionNumber must be 1 or higher.");
    }
}
