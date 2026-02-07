using Fluxor;

namespace BlazorExperiments.Client.Store.Counter;

[FeatureState]
public record CounterState
{
    public int Count { get; init; }
    public bool IsLoading { get; init; }
    public string? LastIncrementedBy { get; init; }

    // Parameterless constructor for Fluxor to create initial state
    public CounterState()
    {
        Count = 0;
        IsLoading = false;
        LastIncrementedBy = null;
    }
}
