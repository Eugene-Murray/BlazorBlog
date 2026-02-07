# Fluxor Setup for Blazor Web App (InteractiveAuto Mode)

## Problem
When using **InteractiveAuto** render mode in a Blazor Web App, you have both:
- **Server-side rendering** (SSR/Interactive Server)
- **Client-side rendering** (WebAssembly)

Components can switch between these modes, so Fluxor must be configured in **both** projects.

## Solution

### 1. Client Project (`Admin.Client/Program.cs`)
```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Admin.Client.Store.Middleware;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

// Add Fluxor for WebAssembly
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(); // Only in WebAssembly
    options.AddMiddleware<LoggingMiddleware>();
});

await builder.Build().RunAsync();
```

### 2. Server Project (`Admin/Program.cs`)
```csharp
using Fluxor;

// ... other usings ...

var builder = WebApplication.CreateBuilder(args);

// ... other services ...

// Add Fluxor for Server-side rendering
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(
        typeof(Program).Assembly,
        typeof(Admin.Client._Imports).Assembly); // Scan client assembly too!
    // Note: UseReduxDevTools() is only for WebAssembly
});

// ... rest of configuration ...
```

### 3. Package References

Both projects need Fluxor packages:

**Client Project (`Admin.Client.csproj`):**
```xml
<PackageReference Include="Fluxor.Blazor.Web" />
<PackageReference Include="Fluxor.Blazor.Web.ReduxDevTools" />
```

**Server Project (`Admin.csproj`):**
```xml
<PackageReference Include="Fluxor.Blazor.Web" />
<PackageReference Include="Fluxor.Blazor.Web.ReduxDevTools" />
```

### 4. StoreInitializer Component

In `Routes.razor`:
```razor
<Fluxor.Blazor.Web.StoreInitializer />

<Router AppAssembly="typeof(Program).Assembly" ...>
    <!-- ... -->
</Router>
```

## Important Notes

### Redux DevTools
- **Only use** `options.UseReduxDevTools()` in the **client** project
- Redux DevTools only work in WebAssembly mode (browser DevTools)
- Don't add it to the server project (it won't work and may cause issues)

### Assembly Scanning
- **Client**: Only scan the client assembly
- **Server**: Scan BOTH server and client assemblies to find all state/reducers/effects

### Why Both Projects?
With `@rendermode InteractiveAuto`:
1. First render happens on the **server** (needs Fluxor on server)
2. Then switches to **WebAssembly** (needs Fluxor on client)
3. If Fluxor is missing on server, you get: `"Cannot provide a value for property 'Store' on type 'Fluxor.Blazor.Web.StoreInitializer'"`

### Middleware
- Custom middleware (like `LoggingMiddleware`) works in both environments
- Be careful with server-specific or browser-specific APIs in middleware

### State Persistence
If you need state to persist when switching from Server to WebAssembly:
- Use session storage or local storage
- Serialize state before switching modes
- Restore state after mode switch

## Debugging Tips

### Check Which Mode Is Running
```csharp
@code {
    [Inject]
    private IWebAssemblyHostEnvironment? HostEnvironment { get; set; }
    
    protected override void OnInitialized()
    {
        if (HostEnvironment != null)
        {
            Console.WriteLine("Running in WebAssembly mode");
        }
        else
        {
            Console.WriteLine("Running in Server mode");
        }
    }
}
```

### Common Errors

**Error**: `Cannot provide a value for property 'Store'`
- **Cause**: Fluxor not configured on server
- **Fix**: Add Fluxor to `Admin/Program.cs`

**Error**: State not found after switching modes
- **Cause**: State is not shared between server and client
- **Fix**: Each mode has its own state instance (by design)

**Error**: Redux DevTools not working
- **Cause**: Only works in WebAssembly mode
- **Fix**: Ensure component is running in WebAssembly mode

## Render Mode Options

### InteractiveServer
```razor
@rendermode InteractiveServer
```
- Only needs Fluxor on server
- State stays on server
- SignalR connection required

### InteractiveWebAssembly
```razor
@rendermode InteractiveWebAssembly
```
- Only needs Fluxor on client
- State in browser memory
- Redux DevTools available

### InteractiveAuto (Current Setup)
```razor
@rendermode InteractiveAuto
```
- Needs Fluxor on **BOTH** server and client
- Starts on server, switches to WebAssembly when downloaded
- Best user experience but more complex setup

## Testing Different Modes

To test server-only behavior:
```razor
@rendermode InteractiveServer
```

To test WebAssembly-only behavior:
```razor
@rendermode InteractiveWebAssembly
```

To test auto-switching behavior:
```razor
@rendermode InteractiveAuto
```
