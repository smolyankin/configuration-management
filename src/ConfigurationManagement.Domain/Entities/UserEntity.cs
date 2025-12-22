using System.ComponentModel.DataAnnotations.Schema;

namespace ConfigurationManagement.Domain.Entities;

/// <summary>
/// Пользователь.
/// </summary>
[Table("users")]
public class UserEntity : BaseEntity
{
    /// <summary>
    /// Почта.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Хешированный пароль в base64.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Конфигурации пользователя.
    /// </summary>
    public virtual ICollection<ConfigurationEntity> Configurations { get; set; } = new List<ConfigurationEntity>();

    /// <summary>
    /// ФИО.
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();
}