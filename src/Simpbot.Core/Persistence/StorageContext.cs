using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Persistence
{
    public class StorageContext : DbContext
    {
        public DbSet<Muted> Muteds { get; set; }

        public DbSet<Prefix> Prefixes { get; set; }
        public DbSet<PrefixDefault> PrefixDefaults { get; set; }

        public async Task<PrefixDefault> GetDefaultPrefix() => await PrefixDefaults.FirstOrDefaultAsync();

        public async Task MigrateAsync()
        {
            await Database.MigrateAsync();
            if (PrefixDefaults.Any())
                return;

            PrefixDefaults.Add(
                new PrefixDefault
                {
                    PrefixSymbol = '.'
                }
            );
            await SaveChangesAsync();

        }

        #region DbContext Overrides

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./simpbot.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Muted>().HasKey(muted => new {muted.GuildId, muted.UserId});
        }

        #endregion

    }
}