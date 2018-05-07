using System.Threading.Tasks;

using NUnit.Framework;

using Simpbot.Core.Dto;
using Simpbot.Service.Search;
using Simpbot.Service.Weather;

namespace Simpbot.Core.Test
{
    public class SendingMessagesTest
    {
        [TestCase(TestName = "Send a message")]
        public async Task SendMessageAsyncTest()
        {
            var client = await CreateBotAndStartAsync();

            client.WaitForConnection().Wait();

            Assert.DoesNotThrowAsync(async () => await client.SendMessage(new Message
                {
                    Text = "Hello there!"
                },
                TestConfiguration.TestGuild));
        }


        private static async Task<Simpbot> CreateBotAndStartAsync()
        {
            var client = new Simpbot(TestConfiguration.BotToken, new WeatherServiceConfiguration(), new SearchServiceConfiguration());
            await client.StartAsync();

            return client;
        }
    }
}