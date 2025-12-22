using ConfigurationManagement.Api.Middleware;

namespace ConfigurationManagement.Api.Extensions;

/// <summary>
/// Расширение обработки ошибок.
/// </summary>
public static class SimpleErrorHandlerExtensions
{
    /// <summary>
    /// Использовать простой обработчик ошибок.
    /// </summary>
    public static IApplicationBuilder UseSimpleErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SimpleErrorHandlerMiddleware>();
    }
}