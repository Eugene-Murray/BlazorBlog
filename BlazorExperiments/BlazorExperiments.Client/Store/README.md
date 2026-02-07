# Fluxor State Management Implementation

This project demonstrates a complete Fluxor state management implementation with the Counter component.

## 📦 What's Included

### 1. **State** (`CounterState.cs`)
- Immutable record-based state with three properties:
  - `Count`: The current counter value
  - `IsLoading`: Loading indicator for async operations
  - `LastIncrementedBy`: Tracks who last incremented the counter
- Decorated with `[FeatureState]` attribute for automatic registration

### 2. **Actions** (`CounterActions.cs`)
Defines all possible actions that can modify the state:

#### Synchronous Actions:
- `IncrementCounterAction`: Increments the counter by 1
- `DecrementCounterAction`: Decrements the counter by 1
- `ResetCounterAction`: Resets the counter to 0

#### Asynchronous Actions (for Effects):
- `IncrementCounterAsyncAction`: Triggers async increment
- `IncrementCounterAsyncSuccessAction`: Handles successful async operation
- `IncrementCounterAsyncFailureAction`: Handles failed async operation

### 3. **Reducers** (`CounterReducers.cs`)
Pure functions that take the current state and an action, then return a new state:
- Each reducer method is decorated with `[ReducerMethod]`
- All reducers use immutable updates with record `with` expressions
- Reducers are automatically discovered and registered by Fluxor

### 4. **Effects** (`CounterEffects.cs`)
Handles side effects and asynchronous operations:
- `HandleIncrementCounterAsync`: Simulates an API call with a 1-second delay
- Uses `IDispatcher` to dispatch success/failure actions
- Includes error handling and logging
- Effects are automatically discovered and registered

### 5. **Middleware** (`LoggingMiddleware.cs`)
Custom middleware that logs all actions:
- Logs when middleware is initialized
- Logs before and after each action dispatch
- Useful for debugging and monitoring state changes
- Registered in `Program.cs`

## 🚀 How to Use

### Setup (Already Done)

1. **Packages Added:**
   ```xml
   <PackageReference Include="Fluxor.Blazor.Web" />
   <PackageReference Include="Fluxor.Blazor.Web.ReduxDevTools" />
   ```

2. **Program.cs Configuration:**
   ```csharp
   builder.Services.AddFluxor(options =>
   {
       options.ScanAssemblies(typeof(Program).Assembly);
       options.UseReduxDevTools();
       options.AddMiddleware<LoggingMiddleware>();
   });
   ```

3. **Routes.razor:**
   ```razor
   <Fluxor.Blazor.Web.StoreInitializer />
   ```

### Using Fluxor in Components

```razor
@inherits Fluxor.Blazor.Web.Components.FluxorComponent

@code {
    [Inject]
    private IState<CounterState> CounterState { get; set; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    private void DoSomething()
    {
        Dispatcher.Dispatch(new SomeAction());
    }
}
```

## 🛠️ Redux DevTools

### Installation
1. Install the Redux DevTools extension for your browser:
   - [Chrome](https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd)
   - [Firefox](https://addons.mozilla.org/en-US/firefox/addon/reduxdevtools/)
   - [Edge](https://microsoftedge.microsoft.com/addons/detail/redux-devtools/nnkgneoiohoecpdiaponcejilbhhikei)

### Using Redux DevTools
1. Run your Blazor app
2. Open browser DevTools (F12)
3. Click on the "Redux" tab
4. You'll see:
   - **Action History**: All dispatched actions in chronological order
   - **State Tree**: Current state of your application
   - **Diff**: Changes made by each action
   - **Time Travel**: Jump to any previous state
   - **Action Details**: Payload of each action

### Features:
- **Time-Travel Debugging**: Click on any action to see the state at that point
- **Action Replay**: Replay actions to reproduce bugs
- **State Import/Export**: Save and load application states
- **Action Filtering**: Filter actions by type
- **State Inspection**: Deep dive into state structure

## 🎯 Best Practices

### 1. **Keep Reducers Pure**
- No side effects
- No API calls
- No random values
- Same input = same output

### 2. **Use Effects for Side Effects**
- API calls
- Local storage
- Logging
- Timers

### 3. **Immutable State Updates**
- Always use `with` expressions for records
- Never mutate state directly
- Create new instances

### 4. **Action Naming**
- Use descriptive names
- Past tense for events: `IncrementedCounter`
- Present tense for commands: `IncrementCounter`
- Include context: `Counter_Incremented`

### 5. **State Organization**
- One feature per state
- Keep state flat when possible
- Use records for immutability
- Initialize with default values

## 📚 Additional Resources

- [Fluxor Documentation](https://github.com/mrpmorris/Fluxor)
- [Redux DevTools Documentation](https://github.com/reduxjs/redux-devtools)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)

## 🔍 Debugging Tips

1. **Check Console Logs**: The LoggingMiddleware logs all actions to the console
2. **Use Redux DevTools**: See the complete action history and state changes
3. **Breakpoints in Effects**: Debug async operations
4. **Inspect State**: Use `IState<T>.Value` to check current state
5. **Action Payload**: Ensure actions carry the right data

## 🧪 Testing

### Testing Reducers
```csharp
[Fact]
public void IncrementReducer_ShouldIncreaseCount()
{
    // Arrange
    var state = new CounterState { Count = 5 };
    var action = new IncrementCounterAction();
    
    // Act
    var newState = CounterReducers.OnIncrementCounter(state, action);
    
    // Assert
    Assert.Equal(6, newState.Count);
}
```

### Testing Effects
```csharp
[Fact]
public async Task IncrementEffect_ShouldDispatchSuccessAction()
{
    // Arrange
    var dispatcher = Substitute.For<IDispatcher>();
    var effect = new CounterEffects(logger);
    var action = new IncrementCounterAsyncAction("Test");
    
    // Act
    await effect.HandleIncrementCounterAsync(action, dispatcher);
    
    // Assert
    dispatcher.Received().Dispatch(Arg.Any<IncrementCounterAsyncSuccessAction>());
}
```
