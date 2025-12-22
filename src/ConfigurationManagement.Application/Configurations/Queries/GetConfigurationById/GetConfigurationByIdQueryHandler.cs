using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Application.Configurations.Queries.GetConfigurationById;

public class GetConfigurationByIdQueryHandler : IRequestHandler<GetConfigurationByIdQuery, ConfigurationDto?>
{
    private readonly ConfigurationDbContext _context;
    private readonly ILogger<GetConfigurationByIdQueryHandler> _logger;

    public GetConfigurationByIdQueryHandler(ConfigurationDbContext context, ILogger<GetConfigurationByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ConfigurationDto?> Handle(GetConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var configuration = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (configuration == null)
        {
            _logger.LogDebug("Configuration not found with ID: {ConfigId}", request.Id);
            return null;
        }
        
        _logger.LogDebug("Found configuration with ID: {ConfigId}", request.Id);

        return new ConfigurationDto
        {
            Id = configuration.Id,
            Name = configuration.Name,
            Data = configuration.Data,
            CreatedAt = configuration.CreatedAt,
            UpdatedAt = configuration.UpdatedAt
        };
    }
}