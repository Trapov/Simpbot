using System.Threading.Tasks;

using Simpbot.Service.Search;
using Simpbot.Service.Weather;

namespace Simpbot.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var bot = new Core.Simpbot(TestConfiguration.BotToken, new WeatherServiceConfiguration
            {
                ApiKey = TestConfiguration.WeatherServiceKey
            }, new SearchServiceConfiguration
            {
                ApiKey = TestConfiguration.ImageServiceKey, CxKey = TestConfiguration.ImageServiceCustomEngineKey
            });

            bot.StartAsync();

            Task.Delay(-1).Wait();
        }
    }
}
