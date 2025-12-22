using MediatR;

namespace ConfigurationManagement.Application.Common.Base;

/// <summary>
/// Интерфейс команды.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Интерфейс команды с возвращаемым значением.
/// </summary>
/// <typeparam name="TResponse">Возвращаемый тип.</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Команда.
/// </summary>
public abstract record Command : ICommand
{
    
}

/// <summary>
/// Команда с возвращаемым значением.
/// </summary>
/// <typeparam name="TResponse">Возвращаемый тип.</typeparam>
public abstract record Command<TResponse> : ICommand<TResponse>
{
    
}