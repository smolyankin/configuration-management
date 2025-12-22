using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Application.NotificationSubscriptions.Commands.Unsubscribe;

public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UnsubscribeCommandHandler(ConfigurationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetCurrentUserIdOrThrows(cancellationToken);

        var subscriptions = await _dbContext.NotificationSubscriptions
            .Where(ns => ns.UserId == currentUserId)
            .ToListAsync(cancellationToken);

        if (subscriptions.Count == 0)
        {
            throw new InvalidOperationException("No active subscription found.");
        }

        _dbContext.RemoveRange(subscriptions);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}