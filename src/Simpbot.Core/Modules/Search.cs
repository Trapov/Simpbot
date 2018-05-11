using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Simpbot.Service.Search;

namespace Simpbot.Core.Modules
{
    public class Search : ModuleBase
    {
        private readonly ISearchService _searchService;
        private readonly ICustomLogger _customLogger;

        public Search(ISearchService searchService, ICustomLogger customLogger)
        {
            _searchService = searchService;
            _customLogger = customLogger;
        }

        [Command("im", RunMode = RunMode.Async), Alias("im2", "image"), Summary("Searches for an image")]
        public async Task SearchForImage([Remainder] string query)
        {
            var result = (await _searchService.SearchForAsync(query, ResultType.Image)).Items?.FirstOrDefault();
            var response = result != null ? result.Link : "No image found!";

            var embed = new EmbedBuilder()
                .WithImageUrl(response)
                .WithColor(Color.Blue)
                .Build();

            await ReplyAsync(Context.User.Mention, false, embed).ConfigureAwait(false);
        }

        [Command("gif", RunMode = RunMode.Async), Summary("Searches for a gif")]
        public async Task SearchForGif([Remainder] string query)
        {
            var result = (await _searchService.SearchForAsync(query, ResultType.Gif)).Items?.FirstOrDefault();
            var response = result != null ? result.Link : "No image found!";

            var embed = new EmbedBuilder()
                .WithImageUrl(response)
                .WithColor(Color.Blue)
                .Build();

            await ReplyAsync(Context.User.Mention, false, embed).ConfigureAwait(false);
        }

        [Command("google", RunMode = RunMode.Async), Summary("Searches for a query")]
        public async Task SearchForTexts([Remainder] string query)
        {
                var response = await _searchService.SearchForAsync(query, ResultType.Text);
                var embed = new EmbedBuilder();

                foreach (var link in response.Items.Take(5))
                    embed
                        .AddField(link.Title, link.Link);

                await ReplyAsync(Context.User.Mention, false, embed.Build()).ConfigureAwait(false);

        }

        [Command("yt", RunMode = RunMode.Async), Summary("Searches for a query")]
        public async Task SearchForVideo([Remainder] string query)
        {

            var response = (await _searchService.SearchForAsync(query, ResultType.Youtube))
                .Items?
                .FirstOrDefault()
                ?.Link;
            //TODO: write a filter exception or something

            await ReplyAsync(response ?? "Video not found").ConfigureAwait(false);

        }
    }
}