using Fluxor;

namespace BlazorExperiments.Client.Store.Middleware;

public class LoggingMiddleware : Fluxor.Middleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public override Task InitializeAsync(IDispatcher dispatcher, IStore store)
    {
        _logger.LogInformation("LoggingMiddleware initialized");
        return Task.CompletedTask;
    }

    public override void AfterDispatch(object action)
    {
        _logger.LogInformation("Action dispatched: {ActionType}", action.GetType().Name);
    }

    public override bool MayDispatchAction(object action)
    {
        _logger.LogDebug("About to dispatch: {ActionType}", action.GetType().Name);
        return true;
    }

    public override void BeforeDispatch(object action)
    {
        _logger.LogDebug("Before dispatch: {ActionType}", action.GetType().Name);
    }
}

