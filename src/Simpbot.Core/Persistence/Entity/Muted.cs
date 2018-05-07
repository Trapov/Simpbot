namespace Simpbot.Core.Persistence.Entity
{
    public class Muted
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public bool IsMuted { get; set; }
    }
}