using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Domain.Exceptions;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Application.Configurations.Queries.GetConfigurationVersions;

public class GetConfigurationVersionsQueryHandler : IRequestHandler<GetConfigurationVersionsQuery, ConfigurationVersionsDto>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ILogger<GetConfigurationVersionsQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public GetConfigurationVersionsQueryHandler(
        ConfigurationDbContext context,
        ILogger<GetConfigurationVersionsQueryHandler> logger)
    {
        _dbContext = context;
        _logger = logger;
    }

    public async Task<ConfigurationVersionsDto> Handle(GetConfigurationVersionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting versions for configuration {ConfigId} with filters: {@Filters}",
            request.ConfigurationId, new
            {
                request.CreatedFrom,
                request.CreatedTo
            });

        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);

        var configuration = await _dbContext.Configurations
            .FirstOrDefaultAsync(c => c.Id == request.ConfigurationId, cancellationToken)
            ?? throw new NotFoundException($"Configuration with ID '{request.ConfigurationId}' not found.");

        if (configuration.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to access this configuration's versions.");
        }

        var query = _dbContext.ConfigurationVersions
            .Where(v => v.ConfigurationId == request.ConfigurationId)
            .AsQueryable();

        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(v => v.CreatedAt >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(v => v.CreatedAt <= request.CreatedTo.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var versions = await query
            .OrderByDescending(v => v.VersionNumber)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => MapToConfigurationVersionDto(v))
            .ToListAsync(cancellationToken);

        // Create pagination info
        var paginationInfo = new PaginationInfo
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };

        _logger.LogDebug("Found {Count} versions for configuration {ConfigId} out of {Total} total",
            versions.Count, request.ConfigurationId, totalCount);

        return new ConfigurationVersionsDto
        {
            Versions = versions,
            Pagination = paginationInfo
        };
    }

    private static ConfigurationVersionDto MapToConfigurationVersionDto(ConfigurationVersionEntity version)
    {
        return new ConfigurationVersionDto
        {
            Id = version.Id,
            ConfigurationId = version.ConfigurationId,
            VersionNumber = version.VersionNumber,
            Name = version.Name,
            CreatedAt = version.CreatedAt,
            Data = version.Data
        };
    }
}