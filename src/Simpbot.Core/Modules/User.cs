using System;
using System.Globalization;
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

        [Command("avatar"), Summary("gives an avatar of a user")]
        public Task Avatar(IUser user)
        {
            try
            {
                
                var embed = new EmbedBuilder()
                    .WithImageUrl(user.GetAvatarUrl())
                    .WithColor(Color.Blue)
                    .Build();

                return ReplyAsync(Context.User.Mention, false, embed);
            }
            catch (Exception e)
            {
                _customLogger.LogAsync(e).Wait();
                return ReplyAsync("Unhandled error");
            }
        }

        [Command("info", RunMode = RunMode.Async), Summary("Gives a user info")]
        public Task GetUserInfoAsync(IUser user)
        {
            var embed = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl(ImageFormat.Png))
                .WithColor(Color.DarkMagenta)
                .AddField("Name:", user.Username)
                .AddField("Joined Discord:", user.CreatedAt.ToString("f", CultureInfo.InvariantCulture))
                .AddField("Current status:", user.Status)
                .AddField("Playing:", user.Activity.Name)
                .Build();

            return ReplyAsync(Context.User.Mention, false, embed);
        }
    }
}