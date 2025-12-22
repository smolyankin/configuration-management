using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Application.Configurations.Queries.GetConfigurations;

public class GetConfigurationsQueryHandler : IRequestHandler<GetConfigurationsQuery, ConfigurationsDto>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ILogger<GetConfigurationsQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public GetConfigurationsQueryHandler(ConfigurationDbContext context,
        ILogger<GetConfigurationsQueryHandler> logger,
        ICurrentUserService currentUserService)
    {
        _dbContext = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<ConfigurationsDto> Handle(GetConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);

        var query = _dbContext.Configurations
            .Where(c => c.UserId == currentUserId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(c => c.Name.Contains(request.Name));
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(c => c.CreatedAt <= request.DateTo.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var configurations = await query
            .OrderBy(c => c.Name)
            .ThenByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => MapToConfigurationDto(v))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        _logger.LogDebug("Found {Count} configurations for user {UserId}, page {PageNumber} of {TotalPages}",
            configurations.Count, currentUserId, request.PageNumber, totalPages);
        
        var paginationInfo = new PaginationInfo
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };

        return new ConfigurationsDto
        {
            Configurations = configurations,
            Pagination = paginationInfo
        };
    }

    private static ConfigurationDto MapToConfigurationDto(ConfigurationEntity configuration)
    {
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