using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Persistence
{
    public class StorageContext : DbContext
    {
        public DbSet<Muted> Muteds { get; set; }
        public DbSet<Prefix> Prefixes { get; set; }
        public DbSet<Quote> Quotes { get; set; }


        public async Task MigrateAsync() => await Database.MigrateAsync();

        #region DbContext Overrides

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./simpbot.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Muted>()
                .HasKey(muted => new {muted.GuildId, muted.UserId});
            modelBuilder.Entity<Quote>()
                .HasKey(quote => new {quote.MessageId, quote.GuildId});

            modelBuilder.Entity<Muted>()
                .HasIndex(muted => muted.UserId);
            modelBuilder.Entity<Quote>()
                .HasIndex(quote => new { quote.MessageId, quote.GuildId, quote.UserId})
                .IsUnique();
        }

        #endregion

    }
}