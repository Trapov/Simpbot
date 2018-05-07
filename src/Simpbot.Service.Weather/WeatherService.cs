using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Simpbot.Service.Weather.Contract;

namespace Simpbot.Service.Weather
{
    public class WeatherService : IWeatherService, IDisposable
    {
        private const string BaseUrl = @"http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}";

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(WeatherServiceConfiguration weatherServiceConfiguration)
        {
            _apiKey = weatherServiceConfiguration.ApiKey;
            _httpClient = new HttpClient();
        }

        public async Task<CityWeather> GetWeather(string city)
        {
            var url = string.Format(BaseUrl, city, _apiKey);
            var result = await _httpClient.GetAsync(url);
            var stringRead = await result.Content.ReadAsStringAsync();
            var jsonMapped = JsonConvert.DeserializeObject<CityWeather>(stringRead);
            return jsonMapped;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}