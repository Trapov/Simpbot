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
    }
}