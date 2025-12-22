using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Domain.Enums;
using ConfigurationManagement.Domain.Exceptions;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Application.Configurations.Commands.UpdateConfiguration;

public class UpdateConfigurationCommandHandler : IRequestHandler<UpdateConfigurationCommand, ConfigurationDto>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotifyService _notifyService;

    public UpdateConfigurationCommandHandler(
        ConfigurationDbContext dbContext,
        ICurrentUserService currentUserService,
        INotifyService notifyService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _notifyService = notifyService;
    }

    public async Task<ConfigurationDto> Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);

        var configuration = await _dbContext.Configurations
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Configuration with ID '{request.Id}' not found.");
        
        if (configuration.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException($"Configuration '{configuration.Name}' does not belong to user '{currentUserId}'.");
        }

        var nextVersionNumber = await _dbContext.ConfigurationVersions
            .Where(v => v.ConfigurationId == configuration.Id)
            .Select(v => (int?)v.VersionNumber)
            .MaxAsync(cancellationToken) + 1 ?? 1;

        var versionSnapshot = new ConfigurationVersionEntity
        {
            ConfigurationId = configuration.Id,
            VersionNumber = nextVersionNumber,
            Name = configuration.Name,
            Data = configuration.Data
        };

        await _dbContext.ConfigurationVersions.AddAsync(versionSnapshot, cancellationToken);

        configuration.Name = request.Name;
        configuration.Data = request.Data;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notifyService.Send(configuration.Id, ConfigurationEventType.Modified, cancellationToken);

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