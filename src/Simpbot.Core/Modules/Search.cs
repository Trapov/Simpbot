using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Microsoft.Extensions.Caching.Memory;

using Simpbot.Service.Search;

namespace Simpbot.Core.Modules
{
    public class Search : ModuleBase
    {
        private readonly ISearchService _searchService;
        private readonly IMemoryCache _cacheService;

        public Search(ISearchService searchService, IMemoryCache cacheService)
        {
            _searchService = searchService;
            _cacheService = cacheService;
        }

        [Command("im", RunMode = RunMode.Async), Alias("im2", "image"), Summary("Searches for an image")]
        public async Task SearchForImage([Remainder] string query)
        {
            var cachedResult = _cacheService.Get<Embed>(query);
            if (cachedResult != null)
            {
                await ReplyAsync(Context.User.Mention, false, cachedResult)
                    .ConfigureAwait(false);
                return;
            }
            
            var result = (await _searchService.SearchForAsync(query, ResultType.Image).ConfigureAwait(false)).Items?.FirstOrDefault();
            var embed = new EmbedBuilder()
                .WithColor(Color.Blue);

            if (result == null)
            {
                await ReplyAsync("No image found").ConfigureAwait(false);
                return;
            }
            embed.WithImageUrl(result.Link);
            var buildEmbed = embed.Build();
            
            _cacheService.Set(query, buildEmbed, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(10)));

            await ReplyAsync(Context.User.Mention, false, buildEmbed).ConfigureAwait(false);
        }

        [Command("gif", RunMode = RunMode.Async), Summary("Searches for a gif")]
        public async Task SearchForGif([Remainder] string query)
        {
            var result = (await _searchService.SearchForAsync(query, ResultType.Gif).ConfigureAwait(false)).Items?.FirstOrDefault();

            var embed = new EmbedBuilder()
                .WithColor(Color.Blue);

            if (result == null)
            {
                await ReplyAsync("No gif found").ConfigureAwait(false);
                return;
            }
            embed.WithImageUrl(result.Link);
            await ReplyAsync(Context.User.Mention, false, embed.Build()).ConfigureAwait(false);
        }

        [Command("google", RunMode = RunMode.Async), Summary("Searches for a query")]
        public async Task SearchForTexts([Remainder] string query)
        {
                var response = await _searchService.SearchForAsync(query, ResultType.Text).ConfigureAwait(false);
                var embed = new EmbedBuilder();

                foreach (var link in response.Items.Take(5))
                    embed
                        .AddField(link.Title, link.Link);

                await ReplyAsync(Context.User.Mention, false, embed.Build()).ConfigureAwait(false);

        }

        [Command("yt", RunMode = RunMode.Async), Summary("Searches for a query")]
        public async Task SearchForVideo([Remainder] string query)
        {
            var response = (await _searchService.SearchForAsync(query, ResultType.Youtube).ConfigureAwait(false))
                .Items?
                .FirstOrDefault()
                ?.Link;

            await ReplyAsync(response ?? "Video not found").ConfigureAwait(false);
        }
    }
}