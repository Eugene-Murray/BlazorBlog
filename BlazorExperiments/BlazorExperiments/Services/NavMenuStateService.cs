namespace BlazorExperiments.Services;

public class NavMenuStateService
{
    public bool IsCollapsed { get; private set; }
    public event Action? OnChange;

    public void SetCollapsed(bool collapsed)
    {
        IsCollapsed = collapsed;
        OnChange?.Invoke();
    }
}
