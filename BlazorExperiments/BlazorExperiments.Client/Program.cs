using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using BlazorExperiments.Client.Store.Middleware;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

// Add Fluxor for WebAssembly
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(); // Redux DevTools only work in WebAssembly
    options.AddMiddleware<LoggingMiddleware>();
});

await builder.Build().RunAsync();
