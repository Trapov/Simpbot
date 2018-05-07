using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Simpbot.Core.Contracts;
using Simpbot.Core.Dto;
using Simpbot.Core.Persistence;
using Simpbot.Core.Persistence.Entity;
using Simpbot.Core.Persistence.Seed;
using Simpbot.Service.Search;
using Simpbot.Service.Weather;
using Simpbot.Service.Wikipedia;

namespace Simpbot.Core
{
    public class Simpbot : ISimpbot
    {
        private readonly string _token;
        private readonly DiscordSocketClient _discordClient;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public Simpbot(string token, WeatherServiceConfiguration weatherServiceConfiguration, SearchServiceConfiguration imageSearchServiceConfiguration)
        {
            _token = token;

            _commandService = new CommandService();

            var services = new ServiceCollection()
                .AddSingleton(provider => weatherServiceConfiguration)
                .AddSingleton(provider => imageSearchServiceConfiguration)
                .AddSingleton(provider => _commandService)
                .AddScoped<IWeatherService, WeatherService>()
                .AddScoped<IWikipediaService, WikipediaService>()
                .AddScoped<ISearchService, SearchService>()
                .AddScoped<ICustomLogger, CustomLogger>()
                .AddDbContext<PrefixContext>(ServiceLifetime.Transient);

            _serviceProvider = services.BuildServiceProvider();

            PrefixSeed.Initialize(_serviceProvider);

#if WINDOWS7
            _discordClient =
                new DiscordSocketClient(new DiscordSocketConfig {WebSocketProvider = WS4NetProvider.Instance});
#else
            _discordClient = new DiscordSocketClient();
#endif
            _discordClient.Log += _serviceProvider
                .GetRequiredService<ICustomLogger>()
                .Log;
        }

        #region Implementation of ISimpbot

        public Task SendMessage(Message message, ulong channelId)
        {
            return InternalSendMessage(message, channelId);
        }

        public async Task StartAsync()
        {
            _discordClient.MessageReceived += HandleCommand;

            await _discordClient.LoginAsync(TokenType.Bot, _token);
            var result = await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly());

            await _discordClient.StartAsync();
        }

        public Task WaitForConnection()
        {
            return Task.Run(() =>
            {
                while (_discordClient.ConnectionState != ConnectionState.Connected)
                {
                    Task.Delay(200);
                }
            });
        }

        #endregion

        #region Helpers

        private async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            if (!(messageParam is SocketUserMessage message)) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            using (var prefixContext = _serviceProvider.GetService<PrefixContext>())
            {
                var guildId = (message.Channel as IGuildChannel)?.Guild.Id;

                var foundPrefix = guildId != null
                    ? (IPrefix) await prefixContext.Prefixes.FirstOrDefaultAsync(prefix => prefix.GuildId.Equals(guildId)) ?? await prefixContext.GetDefaultPrefix()
                    : await prefixContext.GetDefaultPrefix();

                if (
                    !(message.HasCharPrefix(foundPrefix.PrefixSymbol, ref argPos) ||
                        message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))
                ) return;

                // Create a Command Context
                var context = new CommandContext(_discordClient, message);


                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully)
                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private async Task InternalSendMessage(Message message, ulong channelId)
        {
            var channel = _discordClient
                              .GetGuild(channelId)
                              .TextChannels
                              .FirstOrDefault(textChannel => textChannel.Name.Contains("general")) ??
                          throw new SimpbotException("Can't get a channel");
            var result = await channel.SendMessageAsync(message.Text);
        }

        #endregion
    }
}