using FluentValidation;

namespace ConfigurationManagement.Application.NotificationSubscriptions.Commands.Unsubscribe;

/// <summary>
/// Валидатор команды отписки от уведомлений.
/// </summary>
public class UnsubscribeCommandValidator : AbstractValidator<UnsubscribeCommand>
{
    public UnsubscribeCommandValidator()
    {
    }
}