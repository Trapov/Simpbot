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

        public Weather(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [Command("weather", RunMode = RunMode.Async), Summary("Gets the current weather")]
        public async Task GetWeather(string city)
        {
            var result = await _weatherService.GetWeather(city)
                .ConfigureAwait(false);

            var weatherUrl = $@"http://openweathermap.org/img/w/{result.WeatherWeather.FirstOrDefault().Icon}.png";

            var embed = new EmbedBuilder()
                .WithThumbnailUrl(weatherUrl)
                .AddField($"Weather in {result.Name}", $"🌡 {(result.Main.Temp - 273.15).ToString("##.",CultureInfo.InvariantCulture)}°C\n💨 {result.Wind.Speed}km/h\n ")
                .Build();

            await ReplyAsync(Context.User.Mention, false, embed)
                .ConfigureAwait(false);
        }
    }
}