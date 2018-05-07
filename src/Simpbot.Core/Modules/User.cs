using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace Simpbot.Core.Modules
{
    [Group("user")]
    public class User : ModuleBase
    {
        private readonly ICustomLogger _customLogger;

        public User(ICustomLogger customLogger)
        {
            _customLogger = customLogger;
        }

        [Command("avatar", RunMode = RunMode.Async), Summary("gives avatar of a user")]
        public async Task Avatar(IUser user)
        {
            try
            {
                
                var embed = new EmbedBuilder()
                    .WithImageUrl(user.GetAvatarUrl())
                    .WithColor(Color.Blue)
                    .Build();

                await ReplyAsync(Context.User.Mention, false, embed).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await _customLogger.Log(e);
                await ReplyAsync("Unhandled error").ConfigureAwait(false);
            }
        }
    }
}