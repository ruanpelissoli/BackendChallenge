using BackendChallenge.CrossCutting.Common;
using MediatR;

namespace BackendChallenge.CrossCutting.Abstractions;
public interface ICommand : IRequest<Result>, IBaseCommand
{ }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{ }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
where TCommand : ICommand<TResponse>
{ }

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
where TCommand : ICommand
{ }
