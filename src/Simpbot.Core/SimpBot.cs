﻿using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;

using Simpbot.Core.Contracts;
using Simpbot.Core.Dto;
using Simpbot.Core.Persistence;
using Simpbot.Service.Search;
using Simpbot.Service.Weather;
using Simpbot.Service.Wikipedia;
using Simpbot.Core.Extensions;
using Simpbot.Core.Handlers;

using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Simpbot.Core
{
    public class Simpbot : ISimpbot
    {
        private readonly string _token;
        private readonly DiscordSocketClient _discordClient;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        public Action StopCallback { get; set; }

        public Simpbot(Func<SimpbotConfiguration, SimpbotConfiguration> configuration)
        {
            var cnf = configuration.Invoke(new SimpbotConfiguration());

            _token = cnf.Token;
            _commandService = new CommandService();
            var filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) 
                                          + "/Logs/"
                                          + DateTime.Today.ToString("yy-MM-dd")
                                          + ".log";
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Async(sinkConfiguration =>
                    sinkConfiguration.File(filePath));
            if(cnf.LogInConsole)
                loggerConfig = loggerConfig.WriteTo.Async(sinkConfiguration => sinkConfiguration.Console());

            Log.Logger = loggerConfig.CreateLogger();

            _serviceProvider = new ServiceCollection()
                .AddSingleton(provider => cnf.WeatherServiceConfiguration)
                .AddSingleton(provider => cnf.SearchServiceConfiguration)
                .AddSingleton(provider => _commandService)
                .AddScoped<IWeatherService, WeatherService>()
                .AddScoped<IWikipediaService, WikipediaService>()
                .AddScoped<ISearchService, SearchService>()
                .AddScoped<ICustomLogger, CustomLogger>()
                .AddScoped<LoggerAdapter>()
                .AddDbContext<StorageContext>(ServiceLifetime.Transient)
                .AddLogging(builder => builder.AddSerilog(dispose:true))
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
            await Observable.Merge(

                Observable
                    .FromAsync(async () => await _discordClient.LoginAsync(TokenType.Bot, _token))
                    .Select(_ => Unit.Default),
                Observable
                    .FromAsync(async () => await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly()))
                    .Select(_ => Unit.Default),
                Observable
                    .FromAsync(async () => await _serviceProvider.GetService<StorageContext>().MigrateAsync())
                    .Select(_ => Unit.Default),

                Observable.FromEvent<Func<SocketMessage, Task>, SocketMessage>(
                        conversion => arg => Task.Run(() => conversion.Invoke(arg)),
                        h => _discordClient.MessageReceived += h,
                        h => _discordClient.MessageReceived -= h
                    )
                    .SubscribeAsyncChain(new CommandHandler(_serviceProvider, _discordClient, _commandService).HandleCommand)
                    .Select(_ => Unit.Default),
                Observable.FromEvent<Func<Task>, Task>(
                        conversion => (() => Task.Run(() => conversion.Invoke(Task.CompletedTask))),
                        h => _discordClient.Ready += h,
                        h => _discordClient.Ready -= h
                    )
                    .SubscribeAsyncChain(() =>
                        _discordClient.SetActivityAsync(new Game($" in {_discordClient.Guilds.Count} servers")))
                    .Select(_ => Unit.Default),
                Observable.FromEvent<Func<LogMessage, Task>, LogMessage>(
                        conversion => arg => Task.Run(() => conversion.Invoke(arg)),
                        h =>
                        {
                            _discordClient.Log += h;
                            _commandService.Log += h;
                        },
                        h =>
                        {
                            _discordClient.Log -= h;
                            _commandService.Log -= h;
                        })
                    .SubscribeChain(_serviceProvider.GetRequiredService<LoggerAdapter>().Log)
                    .Select(_ => Unit.Default),

                Observable.FromAsync(async unit =>
                {
                    await _discordClient.StartAsync();
                })

            ).Catch<Unit,Exception>( ex =>
            {
                Log.Fatal(ex, "");
                StopCallback?.Invoke();
                Log.CloseAndFlush();
                return Observable.Empty(Unit.Default);
            });
        }

        public Task SendMessage(Message message, ulong channelId)
        {
            return InternalSendMessage(message, channelId);
        }

        #endregion

        #region Implementation of IDisposable

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