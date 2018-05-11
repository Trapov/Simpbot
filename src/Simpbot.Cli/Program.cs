using System.Linq;
using Simpbot.Cli.Service;
using Simpbot.Core.Dto;
using Simpbot.Service.Search;
using Simpbot.Service.Weather;

using BotClient =  Simpbot.Core.Simpbot;

namespace Simpbot.Cli
{
    public static class Program
    {
        private const string RegisterServiceFlag = "--register-service";
        private const string RunAsServiceFlag = "--run-as-service";

        public static void Main(string[] args)
        {
            if (args.Contains(RegisterServiceFlag))
            {
                SimpbotServiceBuilder.RegisterService(RegisterServiceFlag, RunAsServiceFlag);
            }
            else if (args.Contains(RunAsServiceFlag))
            {
                using (var bot = new BotClient(configuration => BuildConfiguration()))
                    SimpbotServiceBuilder.RunAsService(bot);
            }
            else
            {
                using (var bot = new BotClient(configuration => BuildConfiguration()))
                    bot.StartAsync().GetAwaiter().GetResult();
            }
            
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