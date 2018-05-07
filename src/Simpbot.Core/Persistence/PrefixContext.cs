using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Persistence
{
    public class PrefixContext : DbContext
    {
        public DbSet<Prefix> Prefixes { get; set; }
        public DbSet<PrefixDefault> PrefixDefaults { get; set; }

        public async Task<PrefixDefault> GetDefaultPrefix() => await PrefixDefaults.FirstOrDefaultAsync();

        #region DbContext Overrides

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./simpbot.db");
        }

        #endregion

    }
}