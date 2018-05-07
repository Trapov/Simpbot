using System.Threading.Tasks;

using Simpbot.Service.Search.Contracts;

namespace Simpbot.Service.Search
{
    public interface ISearchService
    {
        Task<SearchPayload> SearchForAsync(string query, ResultType result);
    }
}
