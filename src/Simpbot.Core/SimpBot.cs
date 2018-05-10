using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using Simpbot.Core.Contracts;
using Simpbot.Core.Dto;
using Simpbot.Core.Persistence;
using Simpbot.Service.Search;
using Simpbot.Service.Weather;
using Simpbot.Service.Wikipedia;
using Simpbot.Core.Extensions;
using Simpbot.Core.Handlers;

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Simpbot.Core
{
    public class Simpbot : ISimpbot, IDisposable
    {
        private readonly string _token;
        private readonly DiscordSocketClient _discordClient;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public Simpbot(Func<SimpbotConfiguration, SimpbotConfiguration> configuration)
        {
            var cnf = configuration.Invoke(new SimpbotConfiguration());

            _token = cnf.Token;

            _commandService = new CommandService();

            _serviceProvider = new ServiceCollection()
                .AddSingleton(provider => cnf.WeatherServiceConfiguration)
                .AddSingleton(provider => cnf.SearchServiceConfiguration)
                .AddSingleton(provider => _commandService)
                .AddScoped<IWeatherService, WeatherService>()
                .AddScoped<IWikipediaService, WikipediaService>()
                .AddScoped<ISearchService, SearchService>()
                .AddScoped<ICustomLogger, CustomLogger>()
                .AddDbContext<StorageContext>(ServiceLifetime.Transient)
                .BuildServiceProvider();

#if WINDOWS7
            _discordClient =
                new DiscordSocketClient(new DiscordSocketConfig {WebSocketProvider = WS4NetProvider.Instance});
#else
            _discordClient = new DiscordSocketClient();
#endif
        }

        #region Implementation of ISimpbot

        public async Task StartAsync()
        {
            // migrate
            await _serviceProvider.GetService<StorageContext>().MigrateAsync();

            await _discordClient.LoginAsync(TokenType.Bot, _token);
            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly());

            var commandHandler = new CommandHandler(_serviceProvider, _discordClient, _commandService);
            var customLogger = _serviceProvider.GetRequiredService<ICustomLogger>();

            await Observable.Merge(
                Observable.FromEvent<Func<SocketMessage, Task>, SocketMessage>(
                        conversion => arg => Task.Run(() => conversion.Invoke(arg)),
                        h => _discordClient.MessageReceived += h,
                        h => _discordClient.MessageReceived -= h
                    )
                    .SubscribeAsyncChain(commandHandler.HandleCommand)
                    .Select(_ => Unit.Default),
                Observable.FromEvent<Func<LogMessage, Task>, LogMessage>(
                        conversion => arg => Task.Run(() => conversion.Invoke(arg)),
                        h => _discordClient.Log += h,
                        h => _discordClient.Log -= h)
                    .SubscribeAsyncChain(customLogger.LogAsync)
                    .Select(_ => Unit.Default),
                Observable.FromAsync(async unit => await _discordClient.StartAsync())
            );
        }

        public Task SendMessage(Message message, ulong channelId)
        {
            return InternalSendMessage(message, channelId);
        }

        #endregion

        #region IDisposable impl

        public void Dispose()
        {
            _discordClient?.Dispose();
        }

        #endregion

        #region Helpers

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