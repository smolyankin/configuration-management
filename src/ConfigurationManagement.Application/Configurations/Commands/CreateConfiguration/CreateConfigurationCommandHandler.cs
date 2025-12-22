using ConfigurationManagement.Application.Configurations.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Domain.Enums;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Application.Configurations.Commands.CreateConfiguration;

public class CreateConfigurationCommandHandler : IRequestHandler<CreateConfigurationCommand, ConfigurationDto>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotifyService _notifyService;

    public CreateConfigurationCommandHandler(
        ConfigurationDbContext dbContext,
        ICurrentUserService currentUserService,
        INotifyService notifyService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _notifyService = notifyService;
    }

    public async Task<ConfigurationDto> Handle(CreateConfigurationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);

        var existingConfiguration = await _dbContext.Configurations
            .AnyAsync(c => c.UserId == currentUserId && c.Name.ToLower() == request.Name.ToLower(),
                cancellationToken);

        if (existingConfiguration)
        {
            throw new InvalidOperationException($"A configuration with name '{request.Name}' already exists for this user.");
        }

        var configuration = new ConfigurationEntity
        {
            Name = request.Name.Trim(),
            UserId = currentUserId,
            Data = request.Data
        };

        await _dbContext.Configurations.AddAsync(configuration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notifyService.Send(configuration.Id, ConfigurationEventType.Created, cancellationToken);

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