using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Microsoft.Extensions.Caching.Memory;
using Simpbot.Core.Attributes;
using Simpbot.Core.Dto;
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

        [Command("im", RunMode = RunMode.Async), Alias("im2", "image"), Summary("Searches for an image"), Updateable]
        public async Task SearchForImage([Remainder] string query)
        {
            if(await CheckAndReturnFromCache(query).ConfigureAwait(false))
                return;

            var result = (await _searchService.SearchForAsync(query, ResultType.Image).ConfigureAwait(false)).Items?.FirstOrDefault();
            if (result == null)
            {
                await ReplyAsync("No image found").ConfigureAwait(false);
                return;
            }

            var embed = _cacheService.Set(query, new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithImageUrl(result.Link)
                .Build(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(10)));

            // LINK FEATURE
            if (await EditMessageLinkAndReturn(embed).ConfigureAwait(false))
                return;

            _cacheService.Set(Context.Message.Id,
                new MessageLink
                {
                    Caller = Context.Message,
                    Response = await ReplyAsync(Context.User.Mention, false, embed).ConfigureAwait(false)
                }, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            // LINK FEATURE

        }

        [Command("gif", RunMode = RunMode.Async), Summary("Searches for a gif"), Updateable]
        public async Task SearchForGif([Remainder] string query)
        {
            if (await CheckAndReturnFromCache(query).ConfigureAwait(false))
                return;

            var result = (await _searchService.SearchForAsync(query, ResultType.Gif).ConfigureAwait(false)).Items?.FirstOrDefault();
            if (result == null)
            {
                await ReplyAsync("No gif found").ConfigureAwait(false);
                return;
            }

            var embed = _cacheService.Set(query, new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithImageUrl(result.Link)
                .Build(), new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(10)));

            // LINK FEATURE
            if (await EditMessageLinkAndReturn(embed).ConfigureAwait(false))
                return;

            _cacheService.Set(Context.Message.Id,
                new MessageLink
                {
                    Caller = Context.Message,
                    Response = await ReplyAsync(Context.User.Mention, false, embed).ConfigureAwait(false)
                }, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            // LINK FEATURE
        }

        [Command("google", RunMode = RunMode.Async), Summary("Searches for a query"), Updateable]
        public async Task SearchForTexts([Remainder] string query)
        {
            if (await CheckAndReturnFromCache(query).ConfigureAwait(false))
                return;

            var response = await _searchService.SearchForAsync(query, ResultType.Text).ConfigureAwait(false);
            var embed = new EmbedBuilder();

            foreach (var link in response.Items.Take(5))
                embed
                    .AddField(link.Title, link.Link);
            var builded = embed.Build();

            // LINK FEATURE
            if (await EditMessageLinkAndReturn(builded).ConfigureAwait(false))
                return;

            _cacheService.Set(query, builded,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(10)));

            _cacheService.Set(Context.Message.Id,
                new MessageLink
                {
                    Caller = Context.Message,
                    Response = await ReplyAsync(Context.User.Mention, false, builded).ConfigureAwait(false)
                }, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            // LINK FEATURE
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


        #region Helpers

        /// <summary>
        /// checks if it's already in the cache then checks if it has a link, if it was linked then edits the link and returns, if not then just returns from the cache or false
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private async Task<bool> CheckAndReturnFromCache(string query)
        {
            var cachedResult = _cacheService.Get<Embed>(query);
            if (cachedResult == null) return false;

            // LINK FEATURE
            if (await EditMessageLinkAndReturn(cachedResult)) return true;

            await ReplyAsync(Context.User.Mention, false, cachedResult)
                .ConfigureAwait(false);
            return true;

        }

        /// <summary>
        /// edits message if it was linked
        /// </summary>
        /// <param name="embed"></param>
        /// <returns></returns>
        private async Task<bool> EditMessageLinkAndReturn(Embed embed)
        {
            var messageLink = _cacheService.Get<MessageLink>(Context.Message.Id);
            if (messageLink == null || !(messageLink.Response is IUserMessage message)) return false;
            await message.ModifyAsync(properties => { properties.Embed = embed; });
            return true;
        }
        #endregion
    }
}