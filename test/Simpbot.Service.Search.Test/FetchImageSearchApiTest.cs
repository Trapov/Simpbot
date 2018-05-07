using System.Linq;
using NUnit.Framework;

using Simpbot.Service.Search;

using System.Threading.Tasks;


namespace Simpbot.Service.ImageSearch.Test
{
    public class FetchImageSearchApiTest
    {
        [TestCase(TestName = "Fetch a cat image")]
        public async Task FetchCatImageAsync()
        {
            using (var imageSearchService = new SearchService(new SearchServiceConfiguration
            {
                ApiKey = TestConfiguration.ImageServiceKey,
                CxKey = TestConfiguration.ImageServiceCustomEngineKey
            }))
            {
                var result = await imageSearchService.SearchForAsync("Cat", ResultType.Image);

                Assert.NotNull(result);
                Assert.IsTrue(result.Items?.Any() ?? false);
            }
        }
    }
}