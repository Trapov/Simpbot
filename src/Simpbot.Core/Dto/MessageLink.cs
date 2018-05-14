using Discord;

namespace Simpbot.Core.Dto
{
    public class MessageLink
    {
        public IMessage Caller { get; set; }
        public IMessage Response { get; set; }
    }
}