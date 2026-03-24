using EM.CMS.Auth.SqlServer.API.Models;

namespace EM.CMS.Auth.SqlServer.API.Services;

public interface IWeatherForecastService
{
    WeatherForecast[] GetForecast();
}
