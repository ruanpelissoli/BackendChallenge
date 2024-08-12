using BackendChallenge.CrossCutting.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendChallenge.CrossCutting.Middlewares;
public class LoggingMiddleware<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
{
    private readonly ILogger<TRequest> _logger;

    public LoggingMiddleware(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = request!.GetType().Name;

        try
        {
            _logger.LogInformation("Executing {CommandName}", name);

            var result = await next();

            _logger.LogInformation("Executed {CommandName}", name);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {CommandName}", name);

            throw;
        }
    }
}