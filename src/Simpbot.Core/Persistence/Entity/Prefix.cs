using System.ComponentModel.DataAnnotations;

namespace Simpbot.Core.Persistence.Entity
{
    public class Prefix : IPrefix
    {
        [Key]
        public ulong GuildId { get; set; }
        public char PrefixSymbol { get; set; }
    }
}