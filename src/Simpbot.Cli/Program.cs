using System.Threading.Tasks;

using Simpbot.Core.Dto;
using Simpbot.Service.Search;
using Simpbot.Service.Weather;

using BotClient =  Simpbot.Core.Simpbot;

namespace Simpbot.Cli
{
    public static class Program
    {
        public static void Main()
        {
            using (var bot = new BotClient(configuration => BuildConfiguration())) bot.StartAsync().ContinueWith(task => Task.Delay(-1).Wait()).Wait();
        }

        private static SimpbotConfiguration BuildConfiguration() => new SimpbotConfiguration
        {
            Token = BotConfiguration.BotToken,
            WeatherServiceConfiguration = new WeatherServiceConfiguration
            {
                ApiKey = BotConfiguration.WeatherServiceKey
            },
            SearchServiceConfiguration = new SearchServiceConfiguration
            {
                ApiKey = BotConfiguration.ImageServiceKey,
                CxKey = BotConfiguration.ImageServiceCustomEngineKey
            }
        };
    }
}