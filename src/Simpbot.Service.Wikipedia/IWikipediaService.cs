using System.Threading.Tasks;
using Simpbot.Service.Wikipedia.Dto;

namespace Simpbot.Service.Wikipedia
{
    public interface IWikipediaService
    {
        Task<WikipediaPage> SearchForPageAsync(string query);
    }
}