namespace ConfigurationManagement.Domain.Abstractions;

/// <summary>
/// Хеширование пароля.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Захешировать пароль.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Верификация пароля.
    /// </summary>
    bool VerifyPassword(string passwordHash, string providedPassword);
}