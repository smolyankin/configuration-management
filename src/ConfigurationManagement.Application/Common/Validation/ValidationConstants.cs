using System.Text.Json;

namespace ConfigurationManagement.Application.Common.Validation;

/// <summary>
/// Константы валидации.
/// </summary>
public static class ValidationConstants
{
    /// <summary>
    /// Regex имени и фамилии пользователя.
    /// </summary>
    public static readonly string NameRegex = @"^[a-zA-Z\s\-']+$";

    /// <summary>
    /// Regex пароля пользователя.
    /// </summary>
    public static readonly string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).+$";

    /// <summary>
    /// Объект содержит валидный JSON.
    /// </summary>
    public static bool IsValidJson(object data)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(data);
            JsonDocument.Parse(jsonString);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Ограничение размера объекта до 1 МБ.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool WithinSizeLimit(object data)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(data);
            return System.Text.Encoding.UTF8.GetByteCount(jsonString) <= 1024 * 1024; // 1MB
        }
        catch
        {
            return false;
        }
    }
}