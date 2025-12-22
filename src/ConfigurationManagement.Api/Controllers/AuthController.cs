using ConfigurationManagement.Application.Auth.Commands.Login;
using ConfigurationManagement.Application.Auth.Commands.RegisterUser;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationManagement.Api.Controllers;

/// <summary>
/// Регистрация и аутентификация пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user with email {Email}", command.Email);
            return StatusCode(500, "An error occurred while registering the user");
        }
    }

    /// <summary>
    /// Аутентификация пользователя по паролю.
    /// </summary>
    /// <param name="command">Команда для аунтентификации.</param>
    /// <param name="cancellationToken">Токен.</param>
    /// <returns>JWT токен и данные пользователя.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var authResponse = await _mediator.Send(command, cancellationToken);

            return Ok(authResponse);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user with email {Email}", command.Email);
            return StatusCode(500, "An error occurred while processing your login request.");
        }
    }
}