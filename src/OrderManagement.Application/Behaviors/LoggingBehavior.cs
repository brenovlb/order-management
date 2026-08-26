using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OrderManagement.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName =
            typeof(TRequest).Name;

        _logger.LogInformation(
            "Handling {RequestName}",
            requestName);

        Stopwatch stopwatch =
            Stopwatch.StartNew();

        try
        {
            return await next();
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
        }
    }
}