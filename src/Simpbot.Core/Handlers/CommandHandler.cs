using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Simpbot.Core.Persistence;
using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Handlers
{
    public class CommandHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDiscordClient _discordClient;
        private readonly CommandService _commandService;

        public CommandHandler(IServiceProvider serviceProvider, IDiscordClient discordClient, CommandService commandService)
        {
            _serviceProvider = serviceProvider;
            _discordClient = discordClient;
            _commandService = commandService;
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message or it was not a text channel

            if (!(messageParam is SocketUserMessage message)) return;
            if(!(messageParam.Channel is ITextChannel channel)) return;

            // Create a number to track where the prefix ends and the command begins
            using (var storageContext = _serviceProvider.GetService<StorageContext>())
            {
                // MUTED FEATURE
                if (storageContext.Muteds.Any(muted => muted.UserId.Equals(messageParam.Author.Id) && muted.IsMuted))
                {
                    await channel.DeleteMessagesAsync(new[] { messageParam.Id });
                    return;
                }

                var guildId = (message.Channel as IGuildChannel)?.Guild.Id;

                if (guildId == null) return;

                var foundPrefix =
                    (await storageContext.Prefixes.FirstOrDefaultAsync(prefix => prefix.GuildId.Equals(guildId)))?.PrefixSymbol ??
                    Prefix.GetDefaultSymbol();

                var argPos = 0;
                if (
                    !(message.HasCharPrefix(foundPrefix, ref argPos) ||
                      message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))
                ) return;

                // Create a Command Context
                var context = new CommandContext(_discordClient, message);

                await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
            }
        }

        public async Task UpdatedTask(Cacheable<IMessage, ulong> cacheable, SocketMessage socketMessage,
            ISocketMessageChannel channel)
        {
            if (socketMessage == null) return;

            await HandleCommand(socketMessage);
        }
    }
}