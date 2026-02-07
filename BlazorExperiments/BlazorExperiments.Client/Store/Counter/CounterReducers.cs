using Fluxor;

namespace BlazorExperiments.Client.Store.Counter;

public static class CounterReducers
{
    [ReducerMethod]
    public static CounterState OnIncrementCounter(CounterState state, IncrementCounterAction action)
    {
        return state with { Count = state.Count + 1 };
    }

    [ReducerMethod]
    public static CounterState OnDecrementCounter(CounterState state, DecrementCounterAction action)
    {
        return state with { Count = state.Count - 1 };
    }

    [ReducerMethod]
    public static CounterState OnResetCounter(CounterState state, ResetCounterAction action)
    {
        return state with { Count = 0, LastIncrementedBy = null };
    }

    // Async action reducers
    [ReducerMethod]
    public static CounterState OnIncrementCounterAsync(CounterState state, IncrementCounterAsyncAction action)
    {
        return state with { IsLoading = true };
    }

    [ReducerMethod]
    public static CounterState OnIncrementCounterAsyncSuccess(CounterState state, IncrementCounterAsyncSuccessAction action)
    {
        return state with 
        { 
            Count = action.NewCount, 
            IsLoading = false,
            LastIncrementedBy = action.IncrementedBy
        };
    }

    [ReducerMethod]
    public static CounterState OnIncrementCounterAsyncFailure(CounterState state, IncrementCounterAsyncFailureAction action)
    {
        return state with { IsLoading = false };
    }
}
