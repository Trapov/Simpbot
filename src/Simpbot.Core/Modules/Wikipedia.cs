using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Microsoft.Extensions.Caching.Memory;

using Simpbot.Service.Wikipedia;

namespace Simpbot.Core.Modules
{
    public class Wikipedia : ModuleBase
    {
        private readonly IWikipediaService _wikipediaService;
        private readonly IMemoryCache _memoryCache;

        public Wikipedia(IWikipediaService wikipediaService, IMemoryCache memoryCache)
        {
            _wikipediaService = wikipediaService;
            _memoryCache = memoryCache;
        }

        [Command("wiki", RunMode = RunMode.Async), Summary("Gets a wiki page")]
        public async Task GetWikiPage([Remainder] string query)
        {
            var cachedResult = _memoryCache.Get<Embed>(query);
            if (cachedResult != null)
            {
                await ReplyAsync(Context.User.Mention, false, cachedResult)
                    .ConfigureAwait(false);
                return;
            }
            var result = await _wikipediaService.SearchForPageAsync(query)
                .ConfigureAwait(false);

            var embed = new EmbedBuilder()
                .WithTitle(result.Title)
                .WithDescription(result.SelfLink)
                .Build();

            _memoryCache.Set(query, embed);
            
            await ReplyAsync(Context.User.Mention, false, embed)
                .ConfigureAwait(false);
        }
    }
}