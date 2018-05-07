using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Simpbot.Service.Weather;

namespace Simpbot.Core.Modules
{
    public class Weather : ModuleBase
    {
        private readonly IWeatherService _weatherService;
        private readonly ICustomLogger _customLogger;

        public Weather(IWeatherService weatherService, ICustomLogger customLogger)
        {
            _weatherService = weatherService;
            _customLogger = customLogger;
        }

        [Command("weather", RunMode = RunMode.Async), Summary("Gets the current weather")]
        public async Task GetWeather(string city)
        {
            try
            {
                var result = await _weatherService.GetWeather(city);
                var weatherUrl = $@"http://openweathermap.org/img/w/{result.WeatherWeather.FirstOrDefault().Icon}.png";

                var embed = new EmbedBuilder()
                    .WithThumbnailUrl(weatherUrl)
                    .AddField($"Weather in {result.Name}", $"🌡 {(result.Main.Temp - 273.15).ToString("##.",CultureInfo.InvariantCulture)}°C\n💨 {result.Wind.Speed}km/h\n ")
                    .Build();

                await ReplyAsync(Context.User.Mention, false, embed).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _customLogger.Log(e);
                await ReplyAsync("Unhandled error").ConfigureAwait(false);
            }
        }
    }
}