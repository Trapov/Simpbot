using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Microsoft.Extensions.Caching.Memory;

using Simpbot.Service.Weather;

namespace Simpbot.Core.Modules
{
    public class Weather : ModuleBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IMemoryCache _cacheService;

        public Weather(IWeatherService weatherService, IMemoryCache cacheService)
        {
            _weatherService = weatherService;
            _cacheService = cacheService;
        }

        [Command("weather", RunMode = RunMode.Async), Summary("Gets the current weather")]
        public async Task GetWeather(string city)
        {
            var cachedResult = _cacheService.Get<Embed>(city);
            if (cachedResult != null)
            {
                await ReplyAsync(Context.User.Mention, false, cachedResult)
                    .ConfigureAwait(false);
                return;
            }
            
            var result = await _weatherService.GetWeather(city)
                .ConfigureAwait(false);

            if (result == null)
            {
                await ReplyAsync("No such city");
                return;
            }

            var weatherUrl = $@"http://openweathermap.org/img/w/{result.WeatherWeather.FirstOrDefault().Icon}.png";

            var embed = new EmbedBuilder()
                .WithThumbnailUrl(weatherUrl)
                .AddField($"Weather in {result.Name}", $"🌡 {(result.Main.Temp - 273.15).ToString("##.",CultureInfo.InvariantCulture)}°C\n💨 {result.Wind.Speed}km/h\n ")
                .Build();

            _cacheService.Set(city, embed, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(3)));
            
            await ReplyAsync(Context.User.Mention, false, embed)
                .ConfigureAwait(false);
        }
    }
}