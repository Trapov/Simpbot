using System.Threading.Tasks;

using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace Simpbot.Core.Modules
{
    public class Common : ModuleBase
    {
        private readonly ILogger<Common> _logger;

        public Common(ILogger<Common> logger)
        {
            _logger = logger;
        }

        [Command("greenfrog", RunMode = RunMode.Async)]
        public Task GreenFrog()
        {
            return ReplyAsync(@"https://www.youtube.com/watch?v=JybITdCrqyU");
        }
    }
}