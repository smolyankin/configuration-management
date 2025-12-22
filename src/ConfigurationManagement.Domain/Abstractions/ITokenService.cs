namespace ConfigurationManagement.Domain.Abstractions;

/// <summary>
/// Сервис генерации JWT токена.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Сгенерировать токен.
    /// </summary>
    string GenerateToken(Guid userId, string email, string fullName);
}