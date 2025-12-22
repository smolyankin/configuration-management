using MediatR;

namespace ConfigurationManagement.Application.Common.Base;

/// <summary>
/// Интерфейс запроса.
/// </summary>
/// <typeparam name="TResponse">Возвращаемый тип.</typeparam>
public interface IQuery<TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Запрос.
/// </summary>
/// <typeparam name="TResponse">Возвращаемый тип.</typeparam>
public abstract record Query<TResponse> : IQuery<TResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}