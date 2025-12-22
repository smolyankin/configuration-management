namespace ConfigurationManagement.Application.Auth.Dto;

/// <summary>
/// Созданный пользователь.
/// </summary>
public class RegisterUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}