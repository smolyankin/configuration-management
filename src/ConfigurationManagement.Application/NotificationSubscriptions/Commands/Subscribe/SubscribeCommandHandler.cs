using ConfigurationManagement.Application.NotificationSubscriptions.Dto;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Domain.Entities;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Application.NotificationSubscriptions.Commands.Subscribe;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, NotificationSubscriptionDto>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public SubscribeCommandHandler(ConfigurationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<NotificationSubscriptionDto> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);

        var existingSubscription = await _dbContext.NotificationSubscriptions
            .FirstOrDefaultAsync(ns =>
                ns.UserId == currentUserId,
                cancellationToken);

        if (existingSubscription is not null)
        {
            existingSubscription.ConfigurationEventTypes = request.ConfigurationEventTypes;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToDto(existingSubscription);
        }

        var subscription = new NotificationSubscriptionEntity
        {
            UserId = currentUserId,
            ConfigurationEventTypes = request.ConfigurationEventTypes,
        };

        await _dbContext.NotificationSubscriptions.AddAsync(subscription, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(subscription);
    }

    private static NotificationSubscriptionDto MapToDto(NotificationSubscriptionEntity subscription)
    {
        return new NotificationSubscriptionDto
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            EventTypes = subscription.ConfigurationEventTypes.ToList(),
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt
        };
    }
}