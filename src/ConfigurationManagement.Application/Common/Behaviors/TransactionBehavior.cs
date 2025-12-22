using System.Transactions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigurationManagement.Application.Common.Behaviors;

/// <summary>
/// Поведение транзакций.
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!IsCommand(request))
        {
            return await next();
        }

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(30)
            },
            TransactionScopeAsyncFlowOption.Enabled);

        _logger.LogDebug("Starting transaction for command {CommandType}", typeof(TRequest).Name);

        try
        {
            var response = await next();

            transactionScope.Complete();

            _logger.LogDebug("Transaction completed successfully for command {CommandType}", typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for command {CommandType}", typeof(TRequest).Name);
            throw;
        }
    }

    private static bool IsCommand(TRequest request)
    {
        var typeName = request.GetType().Name;
        return typeName.EndsWith("Command") || typeName.EndsWith("Create") || typeName.EndsWith("Update") || typeName.EndsWith("Delete");
    }
}