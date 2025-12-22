using ConfigurationManagement.Application.Auth.Dto;
using MediatR;

namespace ConfigurationManagement.Application.Auth.Commands.RegisterUser;

/// <summary>
/// Команда регистрации нового пользователя с паролем.
/// </summary>
public record RegisterUserCommand : IRequest<RegisterUserDto>
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}