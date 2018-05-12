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

        public Guild(StorageContext guildContext)
        {
            _guildContext = guildContext;
        }

        [Command("mute", RunMode = RunMode.Async)]
        public async Task MuteAsync(IUser user)
        {
            var foundMuted = await _guildContext.Muteds.FirstOrDefaultAsync(muted =>
                muted.GuildId.Equals(Context.Guild.Id) && muted.UserId.Equals(user.Id)).ConfigureAwait(false);
            if (foundMuted != null)
                 foundMuted.IsMuted = true;
            else
                await _guildContext.Muteds.AddAsync(new Muted { UserId = user.Id, GuildId = Context.Guild.Id, IsMuted = true }).ConfigureAwait(false);
            await _guildContext.SaveChangesAsync().ConfigureAwait(false);
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
            await _guildContext.SaveChangesAsync().ConfigureAwait(false);
        }

        [Command("kick", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task KickAsync(IUser user)
        {
            var usr = await Context.Guild.GetUserAsync(user.Id).ConfigureAwait(false);
            await usr.KickAsync().ConfigureAwait(false);
        }

        [Command("prune", RunMode = RunMode.Async), Priority(0)]
        public async Task PruneAsync(IUser user, byte howMany)
        {
            if (howMany++ > 100 || howMany == 0) await ReplyAsync("Can't be bigger than 100 or smaller zero");
            if (Context.Channel is ITextChannel channel)
            {
                var messages =
                    (await channel.GetMessagesAsync().FlattenAsync())
                    .Where(message => message.Author.Id.Equals(user.Id) || message.Id.Equals(Context.Message.Id))
                    .Take(howMany)
                    .ToList();
                await channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
            }
        }

        [Command("prune", RunMode = RunMode.Async), Priority(1)]
        public async Task PruneAsync(byte howMany)
        {
            if (howMany++ > 100 || howMany == 0) await ReplyAsync("Can't be bigger than 100 or smaller zero").ConfigureAwait(false); ;
            if (Context.Channel is ITextChannel channel)
            {
                var messages =
                    (await channel.GetMessagesAsync()
                        .FlattenAsync())
                    .Take(howMany)
                    .ToList();

                await channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
            }
        }
    }
}