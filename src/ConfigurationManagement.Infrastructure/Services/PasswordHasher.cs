using System.Security.Cryptography;
using ConfigurationManagement.Domain.Abstractions;

namespace ConfigurationManagement.Infrastructure.Services;

/// <summary>
/// Хеширование пароля.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        var algorithm = new HashAlgorithmName("SHA256");
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            algorithm,
            KeySize);

        byte[] combinedBytes = new byte[SaltSize + KeySize];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, combinedBytes, SaltSize, KeySize);

        return Convert.ToBase64String(combinedBytes);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string hashedPassword, string password)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(password))
            return false;

        var algorithm = new HashAlgorithmName("SHA256");

        try
        {
            byte[] combinedBytes = Convert.FromBase64String(hashedPassword);

            if (combinedBytes.Length != SaltSize + KeySize)
                return false;

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(combinedBytes, 0, salt, 0, SaltSize);

            byte[] storedHash = new byte[KeySize];
            Buffer.BlockCopy(combinedBytes, SaltSize, storedHash, 0, KeySize);

            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                algorithm,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}