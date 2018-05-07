using System.Threading.Tasks;
using NUnit.Framework;

namespace Simpbot.Service.Weather.Test
{
    public class FetchingWeatherApiTest
    {
        [TestCase(TestName = "Fetch the weather about London")]
        public async Task FetchTestAsync()
        {
            using (var service = new WeatherService(new WeatherServiceConfiguration
            {
                ApiKey = TestConfiguration.WeatherServiceKey
            }))
            {
                var result = await service.GetWeather("London");

                Assert.NotNull(result);
                Assert.AreEqual("London", result.Name);
            }
        }
    }
}