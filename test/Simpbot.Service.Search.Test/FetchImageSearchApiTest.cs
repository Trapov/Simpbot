using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Simpbot.Service.Search.Test
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