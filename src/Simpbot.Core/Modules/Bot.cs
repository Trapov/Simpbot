﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Simpbot.Core.Persistence;
using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Modules
{
    [Group("bot")]
    public class Bot : ModuleBase
    {
        private readonly CommandService _commandService;
        private readonly StorageContext _prefixContext;
        private readonly ICustomLogger _customLogger;
        private readonly ILogger<Bot> _logger;

        public Bot(CommandService commandService, StorageContext prefixContext, ICustomLogger customLogger, ILogger<Bot> logger)
        {
            _commandService = commandService;
            _prefixContext = prefixContext;
            _customLogger = customLogger;
            _logger = logger;
        }

        [Command("info", RunMode = RunMode.Async), Summary("info about the bot")]
        public async Task InfoAsync()
        {
            var prefix = (await _prefixContext.Prefixes.FindAsync(Context.Guild.Id)
                             .ConfigureAwait(false)
                         )?.PrefixSymbol ?? Prefix.GetDefaultSymbol();

            var response = new StringBuilder()
                .AppendLine("**Available Commands:**")
                .AppendLine("```");

            foreach (var commandServiceCommand in _commandService
                .Commands
                .OrderBy(csc => string.IsNullOrEmpty(csc.Aliases.FirstOrDefault())
                    ? csc.Name
                    : csc.Aliases.FirstOrDefault() + " "))
            {
                var prefixGroup = commandServiceCommand.Aliases.FirstOrDefault();

                var commandName =
                    string.IsNullOrEmpty(prefixGroup)
                    ? commandServiceCommand.Name
                    : prefixGroup + " ";

                response.AppendLine(
                    $"{prefix}{commandName}{string.Join(" ", commandServiceCommand.Parameters.Select(s => "<" + s + ">"))}"
                        .PadRight(25, ' ')
                    + (commandServiceCommand.Summary != null ? $"- {commandServiceCommand.Summary}" : ""));
            }
            response.AppendLine("```");

            var embed = new EmbedBuilder()
                .WithColor(Color.DarkPurple)
                .WithDescription(response.ToString())
                .Build();

            await ReplyAsync(Context.User.Mention, false, embed)
                .ConfigureAwait(false);

        }

        [Command("prefix", RunMode = RunMode.Async), Summary("updates the prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PrefixAsync(char newPrefix)
        {
            var foundPrefix = await (
                from prefix in _prefixContext.Prefixes
                where prefix.GuildId.Equals(Context.Guild.Id)
                select prefix
            ).FirstOrDefaultAsync().ConfigureAwait(false);

            if (foundPrefix != null)
                foundPrefix.PrefixSymbol = newPrefix;
            else
                _prefixContext.Prefixes.Add(new Prefix
                {
                    GuildId = Context.Guild.Id,
                    PrefixSymbol = newPrefix
                });

            await _prefixContext.SaveChangesAsync().ConfigureAwait(false);

            await ReplyAsync(Context.User.Mention)
                .ConfigureAwait(false);
        }
    }
}