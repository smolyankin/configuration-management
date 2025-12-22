using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Application.Common.Behaviors;

/// <summary>
/// Поведение логирования с расширенными возможностями.
/// </summary>
public class EnhancedLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<EnhancedLoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ActivitySource _activitySource;

    public EnhancedLoggingBehavior(ILogger<EnhancedLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _activitySource = new ActivitySource("ConfigurationManagement.Application");
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();

        using var activity = _activitySource.StartActivity($"{requestName} Processing");
        activity?.SetTag("request.name", requestName);
        activity?.SetTag("request.id", requestId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Starting request {RequestName} with ID {RequestId}.",
                requestName, requestId);

            var response = await next();

            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation(
                "Completed request {RequestName} with ID {RequestId} in {ElapsedMs}ms.",
                requestName, requestId, elapsed);

            activity?.SetStatus(ActivityStatusCode.Ok);
            activity?.SetTag("duration.ms", elapsed);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;

            _logger.LogError(ex,
                "Failed request {RequestName} with ID {RequestId} after {ElapsedMs}ms. Error: {ErrorType} - {ErrorMessage}",
                requestName, requestId, elapsed, ex.GetType().Name, ex.Message);

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("error.type", ex.GetType().Name);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetTag("duration.ms", elapsed);

            throw;
        }
    }
}