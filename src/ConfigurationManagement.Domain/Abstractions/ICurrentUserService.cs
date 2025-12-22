using System.Security.Claims;

namespace ConfigurationManagement.Domain.Abstractions;

/// <summary>
/// Сервис текущего пользователя.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Получить ID текущего пользователя или кинуть ошибку.
    /// </summary>
    Task<Guid> GetCurrentUserIdOrThrows(CancellationToken cancellationToken);

    /// <summary>
    /// Получить ID текущего пользователя.
    /// </summary>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Получить текущего пользователя.
    /// </summary>
    /// <returns></returns>
    ClaimsPrincipal? GetCurrentUser();
}