using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Domain.Enums;
using ConfigurationManagement.Domain.Exceptions;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Application.Configurations.Commands.RestoreConfigurationVersion;

public class RestoreConfigurationVersionCommandHandler : IRequestHandler<RestoreConfigurationVersionCommand, ConfigurationDto>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotifyService _notifyService;

    public RestoreConfigurationVersionCommandHandler(
        ConfigurationDbContext dbContext,
        ICurrentUserService currentUserService,
        INotifyService notifyService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _notifyService = notifyService;
    }

    public async Task<ConfigurationDto> Handle(RestoreConfigurationVersionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);
        
        var configuration = await _dbContext.Configurations
            .FirstOrDefaultAsync(c => c.Id == request.ConfigurationId, cancellationToken)
            ?? throw new NotFoundException($"Configuration with ID '{request.ConfigurationId}' not found.");

        if (configuration.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException($"Configuration '{configuration.Name}' does not belong to user '{currentUserId}'.");
        }

        var targetVersion = await _dbContext.ConfigurationVersions
            .Where(v => v.ConfigurationId == request.ConfigurationId &&
                v.VersionNumber == request.VersionNumber)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Version {request.VersionNumber} not found for configuration '{request.ConfigurationId}'.");

        var nextVersionNumber = await _dbContext.ConfigurationVersions
            .Where(v => v.ConfigurationId == configuration.Id)
            .Select(v => (int?)v.VersionNumber)
            .MaxAsync(cancellationToken) + 1 ?? 1;

        var preRestoreVersion = new ConfigurationVersionEntity
        {
            ConfigurationId = configuration.Id,
            VersionNumber = nextVersionNumber,
            Name = configuration.Name,
            Data = configuration.Data,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.ConfigurationVersions.AddAsync(preRestoreVersion, cancellationToken);

        configuration.Name = targetVersion.Name;
        configuration.Data = targetVersion.Data;    
        configuration.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notifyService.Send(configuration.Id, ConfigurationEventType.Restored, cancellationToken);

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