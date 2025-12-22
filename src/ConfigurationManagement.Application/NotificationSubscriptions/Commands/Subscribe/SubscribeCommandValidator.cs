using ConfigurationManagement.Domain.Enums;
using FluentValidation;

namespace ConfigurationManagement.Application.NotificationSubscriptions.Commands.Subscribe;

/// <summary>
/// Валидатор команды подписки на уведомления.
/// </summary>
public class SubscribeCommandValidator : AbstractValidator<SubscribeCommand>
{
    public SubscribeCommandValidator()
    {
        RuleFor(x => x.ConfigurationEventTypes)
            .Must(x => x.All(e => Enum.IsDefined(typeof(ConfigurationEventType), e)))
            .When(x => x.ConfigurationEventTypes.Any())
            .WithMessage("One or more ConfigurationEventTypes are invalid.");
    }
}