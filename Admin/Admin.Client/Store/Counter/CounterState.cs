using Fluxor;

namespace Admin.Client.Store.Counter;

[FeatureState]
public record CounterState
{
    public int Count { get; init; }
    public bool IsLoading { get; init; }
    public string? LastIncrementedBy { get; init; }

    public CounterState()
    {
        Count = 0;
        IsLoading = false;
    }

    private CounterState(int count, bool isLoading, string? lastIncrementedBy)
    {
        Count = count;
        IsLoading = isLoading;
        LastIncrementedBy = lastIncrementedBy;
    }
}
