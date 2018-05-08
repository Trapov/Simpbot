using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Microsoft.EntityFrameworkCore;

using Simpbot.Core.Persistence;
using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Modules
{
    public class Guild : ModuleBase
    {
        private readonly StorageContext _guildContext;
        private readonly ICustomLogger _customLogger;

        public Guild(StorageContext guildContext, ICustomLogger customLogger)
        {
            _guildContext = guildContext;
            _customLogger = customLogger;
        }

        [Command("mute", RunMode = RunMode.Async)]
        public async Task MuteAsync(IUser user)
        {
            var foundMuted = await _guildContext.Muteds.FirstOrDefaultAsync(muted =>
                muted.GuildId.Equals(Context.Guild.Id) && muted.UserId.Equals(user.Id));
            if (foundMuted != null)
                 foundMuted.IsMuted = true;
            else
                await _guildContext.Muteds.AddAsync(new Muted { UserId = user.Id, GuildId = Context.Guild.Id, IsMuted = true });
            await _guildContext.SaveChangesAsync();
        }

        [Command("unmute", RunMode = RunMode.Async)]
        public async Task UnMuteAsync(IUser user)
        {
            var muted = await ( 
                                from usr in _guildContext.Muteds
                                where usr.UserId.Equals(user.Id) &&
                                      usr.GuildId.Equals(Context.Guild.Id)
                                select usr
                    )
                .FirstOrDefaultAsync();

            if(muted == null) return;

            muted.IsMuted = false;
            await _guildContext.SaveChangesAsync();
        }

        [Command("kick", RunMode = RunMode.Async)]
        public async Task KickAsync(IUser user)
        {
            var usr = await Context.Guild.GetUserAsync(user.Id);
            await usr.KickAsync();
        }

        [Command("prune", RunMode = RunMode.Async)]
        [Priority(0)]
        public async Task PruneAsync(IUser user, int howMany)
        {
            howMany = howMany == 1 && user.Id.Equals(Context.User.Id) ? 2 : howMany;
            try
            {
                if (Context.Channel is ITextChannel channel)
                {
                    var messages =
                        (await channel.GetMessagesAsync(howMany).FlattenAsync())
                        .Where(message => message.Author.Id.Equals(user.Id));
                    await channel.DeleteMessagesAsync(messages);
                }
            }
            catch (Exception e)
            {
                await _customLogger.LogAsync(e);
                throw;
            }

        }

        [Command("prune", RunMode = RunMode.Async)]
        [Priority(1)]
        public async Task PruneAsync(int howMany)
        {
            howMany = howMany == 1 ? 2 : howMany;
            try
            {
                if (Context.Channel is ITextChannel channel)
                {
                    var messages =
                        await channel.GetMessagesAsync(howMany).FlattenAsync();
                    await channel.DeleteMessagesAsync(messages);
                }
            }
            catch (Exception e)
            {
                await _customLogger.LogAsync(e);
                throw;
            }

        }
    }
}