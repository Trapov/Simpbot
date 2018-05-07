using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Simpbot.Service.Wikipedia.Contract;
using Simpbot.Service.Wikipedia.Dto;

namespace Simpbot.Service.Wikipedia
{
    public class WikipediaService : IWikipediaService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = @"https://en.wikipedia.org/w/api.php?action=query&format=json";

        
        public WikipediaService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WikipediaPage> SearchForPageAsync(string query)
        {
            var escapedString = Regex.Escape(query);

            var result = await _httpClient.GetStringAsync(ApiEndpoint + $"&list=search&srsearch={escapedString}");

            var mappedResult = JsonConvert.DeserializeObject<WikipediaResult>(result).Query.Search.FirstOrDefault();
            
            return new WikipediaPage
            {
                Title = mappedResult.Title,
                SelfLink = string.Format("https://en.wikipedia.org/?curid={0}", mappedResult.Pageid)
            };

        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}