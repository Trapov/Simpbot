using Simpbot.Service.Search;
using Simpbot.Service.Weather;

namespace Simpbot.Core.Dto
{
    public class SimpbotConfiguration
    {
        public string Token { get; set; }
        public WeatherServiceConfiguration WeatherServiceConfiguration { get; set; }
        public SearchServiceConfiguration SearchServiceConfiguration { get; set; }
    }
}