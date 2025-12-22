using System.Net;

namespace ConfigurationManagement.Api.Middleware;

/// <summary>
/// Обработчик ошибок.
/// </summary>
public class SimpleErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SimpleErrorHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public SimpleErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<SimpleErrorHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request");
            await HandleExceptionAsync(context, ex, _environment);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment environment)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            error = new
            {
                code = "INTERNAL_SERVER_ERROR",
                message = environment.IsDevelopment() ? exception.Message : "An internal server error occurred",
                timestamp = DateTime.UtcNow
            }
        };

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}