using ConfigurationManagement.Application.Auth.Dto;
using MediatR;

namespace ConfigurationManagement.Application.Auth.Commands.Login;

/// <summary>
/// Команда аутентификации пользователя по паролю.
/// </summary>
public record LoginCommand : IRequest<AuthDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}