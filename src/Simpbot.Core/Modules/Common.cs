using System.Threading.Tasks;

using Discord.Commands;

namespace Simpbot.Core.Modules
{
    public class Common : ModuleBase
    {
        [Command("greenfrog", RunMode = RunMode.Async)]
        public Task GreenFrog()
        {
            return ReplyAsync(@"https://www.youtube.com/watch?v=JybITdCrqyU");
        }
    }
}