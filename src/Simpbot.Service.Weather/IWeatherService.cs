using System.Threading.Tasks;

using Simpbot.Service.Weather.Contract;

namespace Simpbot.Service.Weather
{
    public interface IWeatherService
    {
        Task<CityWeather> GetWeather(string city);
    }
}