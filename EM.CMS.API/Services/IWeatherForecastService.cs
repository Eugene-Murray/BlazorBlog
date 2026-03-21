using EM.CMS.API.Models;

namespace EM.CMS.API.Services;

public interface IWeatherForecastService
{
    WeatherForecast[] GetForecast();
}
