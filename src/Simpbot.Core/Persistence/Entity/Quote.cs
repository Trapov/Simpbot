using System;

namespace Simpbot.Core.Persistence.Entity
{
    public class Quote
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public ulong MessageId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset DateTime { get; set; }
    }
}