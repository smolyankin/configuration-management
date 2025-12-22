using ConfigurationManagement.Application.Configurations.Commands.CreateConfiguration;
using ConfigurationManagement.Application.Configurations.Commands.RestoreConfigurationVersion;
using ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;
using ConfigurationManagement.Application.Configurations.Queries.GetConfigurationById;
using ConfigurationManagement.Application.Configurations.Queries.GetConfigurations;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationManagement.Api.Controllers;

/// <summary>
/// Конфигурации пользователей.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ConfigurationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConfigurationsController> _logger;

    public ConfigurationsController(IMediator mediator, ILogger<ConfigurationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Создать новую конфигурацию.
    /// </summary>
    /// <param name="command">Команда создания конфигурации.</param>
    /// <param name="cancellationToken">Токен.</param>
    /// <returns>201</returns>
    [HttpPost]
    public async Task<IActionResult> CreateConfiguration([FromBody] CreateConfigurationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var configuration = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetConfiguration), new { id = configuration.Id }, configuration);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating configuration");
            return StatusCode(500, "An error occurred while creating the configuration");
        }
    }

    /// <summary>
    /// Получить конфигурации пользователя с пагинацией.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetConfigurations(
        [FromQuery] GetConfigurationsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configurations for user");
            return StatusCode(500, "An error occurred while retrieving configurations");
        }
    }

    /// <summary>
    /// Get configuration by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetConfiguration(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var query = new GetConfigurationByIdQuery
            {
                Id = id
            };

            var configuration = await _mediator.Send(query, cancellationToken);

            if (configuration == null)
            {
                return NotFound();
            }

            return Ok(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration with ID {ConfigId}", id);
            return StatusCode(500, "An error occurred while retrieving the configuration");
        }
    }

    /// <summary>
    /// Обновить существующую конфигурацию (создает новую версию).
    /// </summary>
    /// <param name="id">ID конфигурации.</param>
    /// <param name="command">Команда обновления конфигурации.</param>
    /// <param name="cancellationToken">Токен.</param>
    /// <returns>200</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateConfiguration(Guid id, [FromBody] UpdateConfigurationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            command = command with { Id = id };
            var configuration = await _mediator.Send(command, cancellationToken);

            return Ok(configuration);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration with ID {ConfigId}", id);
            return StatusCode(500, "An error occurred while updating the configuration");
        }
    }

    /// <summary>
    /// Восстановить конфигурацию к указанной версии.
    /// </summary>
    /// <param name="configurationId">ID конфигурации.</param>
    /// <param name="command">Команда восстановления версии конфигурации.</param>
    /// <param name="cancellationToken">Токен.</param>
    /// <returns>200</returns>
    [HttpPost("{configurationId:guid}/restore")]
    public async Task<IActionResult> RestoreConfigurationVersion(Guid configurationId, [FromBody] RestoreConfigurationVersionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            command = command with
            {
                ConfigurationId = configurationId
            };
            var configuration = await _mediator.Send(command, cancellationToken);

            return Ok(configuration);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring configuration {ConfigId} to version", configurationId);
            return StatusCode(500, "An error occurred while restoring the configuration");
        }
    }
}

