using System.Threading.Tasks;

using Simpbot.Service.Search;
using Simpbot.Service.Weather;

namespace Simpbot.Cli
{
    public static class Program
    {
        public static void Main()
        {
            var configWeather = new WeatherServiceConfiguration
            {
                ApiKey = TestConfiguration.WeatherServiceKey
            };
            var configSearch = new SearchServiceConfiguration
            {
                ApiKey = TestConfiguration.ImageServiceKey,
                CxKey = TestConfiguration.ImageServiceCustomEngineKey
            };

            using (var bot = new Core.Simpbot(TestConfiguration.BotToken, configWeather, configSearch))
            {
                bot.StartAsync();
                Task.Delay(-1).Wait();
            }
        }
    }
}