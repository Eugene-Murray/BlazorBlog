using Fluxor;

namespace Admin.Client.Store.Counter;

public class CounterEffects
{
    private readonly ILogger<CounterEffects> _logger;

    public CounterEffects(ILogger<CounterEffects> logger)
    {
        _logger = logger;
    }

    [EffectMethod]
    public async Task HandleIncrementCounterAsync(IncrementCounterAsyncAction action, IDispatcher dispatcher)
    {
        try
        {
            _logger.LogInformation("Incrementing counter asynchronously for: {IncrementedBy}", action.IncrementedBy);
            
            // Simulate an async operation (e.g., API call)
            await Task.Delay(1000);
            
            // Simulate getting a new count from a server
            var newCount = Random.Shared.Next(1, 100);
            
            dispatcher.Dispatch(new IncrementCounterAsyncSuccessAction(newCount, action.IncrementedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing counter");
            dispatcher.Dispatch(new IncrementCounterAsyncFailureAction(ex.Message));
        }
    }
}
