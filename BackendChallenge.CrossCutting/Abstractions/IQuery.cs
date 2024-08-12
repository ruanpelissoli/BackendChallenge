using BackendChallenge.CrossCutting.Common;
using MediatR;

namespace BackendChallenge.CrossCutting.Abstractions;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{ }

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
where TQuery : IQuery<TResponse>
{ }
