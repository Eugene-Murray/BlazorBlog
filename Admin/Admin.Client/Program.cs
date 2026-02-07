using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Admin.Client.Store.Middleware;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

// Add Fluxor
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseReduxDevTools(rdt =>
    {
        rdt.Name = "My application";
        rdt.EnableStackTrace();
    });
    options.AddMiddleware<LoggingMiddleware>();
});

await builder.Build().RunAsync();
