using EM.CMS.API.Models;
using EM.CMS.API.Services;

namespace EM.CMS.API.Endpoints;

public static class WeatherForecastEndpoints
{
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/weatherforecast")
            .WithTags("Weather")
            .RequireAuthorization();

        group.MapGet("/", (IWeatherForecastService service) => service.GetForecast())
            .WithName("GetWeatherForecast")
            .WithOpenApi();

        return app;
    }
}
