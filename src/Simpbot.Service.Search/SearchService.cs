using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Simpbot.Service.Search.Contracts;

namespace Simpbot.Service.Search
{
    public class SearchService : ISearchService, IDisposable
    {
        private const string BaseImageUrl = @"https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&searchType=image&fileType=jpg&imgSize=xlarge&alt=json";
        private const string BaseTextUrl = @"https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}";
        private const string BaseGifUrl = @"https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}&searchType=image&fileType=gif&imgSize=xlarge&alt=json";
        private const string BaseYoutubeUrl = @"https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2} site:youtube.com";

        private readonly string _apiKey;
        private readonly string _cxKey;
        private readonly HttpClient _httpClient;

        public SearchService(SearchServiceConfiguration searchServiceConfiguration)
        {
            _apiKey = searchServiceConfiguration.ApiKey;
            _cxKey = searchServiceConfiguration.CxKey;

            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public async Task<SearchPayload> SearchForAsync(string query, ResultType result)
        {
            switch (result)
            {
                case ResultType.Text:
                    var requestTextUrl = string.Format(BaseTextUrl, _apiKey, _cxKey, query);

                    return JsonConvert.DeserializeObject<SearchPayload>(await (await _httpClient.GetAsync(requestTextUrl)).Content.ReadAsStringAsync());
                case ResultType.Image:
                    var requestImgUrl = string.Format(BaseImageUrl, _apiKey, _cxKey, query);

                    return JsonConvert.DeserializeObject<SearchPayload>(await (await _httpClient.GetAsync(requestImgUrl)).Content.ReadAsStringAsync());
                case ResultType.Youtube:
                    var requestVideoUrl = string.Format(BaseYoutubeUrl, _apiKey, _cxKey, query);

                    return JsonConvert.DeserializeObject<SearchPayload>(await (await _httpClient.GetAsync(requestVideoUrl)).Content.ReadAsStringAsync());
                case ResultType.Gif:
                    var requestGifUrl = string.Format(BaseGifUrl, _apiKey, _cxKey, query);

                    return JsonConvert.DeserializeObject<SearchPayload>(await (await _httpClient.GetAsync(requestGifUrl)).Content.ReadAsStringAsync());
                default:
                    throw new ArgumentException($"Can't find {result}");
            }

        }
    }
}