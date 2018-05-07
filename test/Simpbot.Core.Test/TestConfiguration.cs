using Microsoft.Extensions.Configuration;

namespace Simpbot.Core.Test
{
    public static class TestConfiguration
    {
        public static string BotToken { get; }
        public static ulong TestGuild { get; }


        static TestConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("secret.secrets.json")
                .Build();

            BotToken = configuration.GetSection("Secret:BotToken").Value;
            TestGuild = ulong.Parse(configuration.GetSection("Secret:TestGuild").Value);
        }
    }
}

