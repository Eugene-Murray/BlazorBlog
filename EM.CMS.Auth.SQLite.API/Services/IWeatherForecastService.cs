using EM.CMS.Auth.SQLite.API.Models;

namespace EM.CMS.Auth.SQLite.API.Services;

public interface IWeatherForecastService
{
    WeatherForecast[] GetForecast();
}
