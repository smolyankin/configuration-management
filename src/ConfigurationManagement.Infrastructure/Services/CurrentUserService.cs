using System.Security.Claims;
using ConfigurationManagement.Domain.Abstractions;
using ConfigurationManagement.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationManagement.Infrastructure.Services;

/// <summary>
/// Сервис текущего пользователя.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ConfigurationDbContext _dbContext;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor,
        ConfigurationDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = GetCurrentUser()?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <inheritdoc />
    public async Task<Guid> GetCurrentUserIdOrThrows(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var userExists = await _dbContext.Users
            .AnyAsync(u => u.Id == currentUserId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException($"User with ID '{currentUserId}' does not exist.");
        }

        return currentUserId;
    }

    /// <inheritdoc />
    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}