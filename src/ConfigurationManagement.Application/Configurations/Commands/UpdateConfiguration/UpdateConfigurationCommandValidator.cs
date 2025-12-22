using ConfigurationManagement.Application.Common.Validation;
using FluentValidation;

namespace ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;

/// <summary>
/// Валидатор изменения существующей конфигурации.
/// </summary>
public class UpdateConfigurationCommandValidator : AbstractValidator<UpdateConfigurationCommand>
{
    public UpdateConfigurationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Configuration name is required.")
            .MaximumLength(255)
            .WithMessage("Configuration name cannot exceed 255 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-_.]+$")
            .WithMessage("Configuration name can only contain letters, numbers, spaces, hyphens, underscores, and periods.");

        RuleFor(x => x.Data)
            .Must(ValidationConstants.IsValidJson)
            .WithMessage("Configuration data must be valid JSON.")
            .Must(ValidationConstants.WithinSizeLimit)
            .WithMessage("Configuration data cannot exceed 1MB in size.");
    }
}
