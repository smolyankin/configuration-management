using ConfigurationManagement.Application.NotificationSubscriptions.Commands.Subscribe;
using ConfigurationManagement.Application.NotificationSubscriptions.Commands.Unsubscribe;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationManagement.Api.Controllers;

/// <summary>
/// Контроллер для управления подписками на уведомления.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NotificationSubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationSubscriptionsController> _logger;

    public NotificationSubscriptionsController(
        IMediator mediator,
        ILogger<NotificationSubscriptionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Подписаться на уведомления о изменениях конфигураций.
    /// </summary>
    /// <param name="command">Команда подключения подписки.</param>
    /// <param name="cancellationToken">Токен.</param>
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(
        [FromBody] SubscribeCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var subscription = await _mediator.Send(command, cancellationToken);

            return Ok(subscription);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification subscription");
            return StatusCode(500, "An error occurred while creating the subscription");
        }
    }

    /// <summary>
    /// Отписаться от уведомлений о изменениях конфигураций.
    /// </summary>
    /// <param name="command">Команда отключения подписки.</param>
    /// <param name="cancellationToken">Токен.</param>
    /// <returns>No content</returns>
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe(
        [FromBody] UnsubscribeCommand command,
        CancellationToken cancellationToken = default)
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
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing notification subscription");
            return StatusCode(500, "An error occurred while removing the subscription");
        }
    }
}