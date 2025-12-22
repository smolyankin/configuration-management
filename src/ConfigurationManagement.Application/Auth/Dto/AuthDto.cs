namespace ConfigurationManagement.Application.Auth.Dto;

/// <summary>
/// Аутентификационные данные пользователя.
/// </summary>
public class AuthDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}