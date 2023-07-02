using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class MalContext : DbContext
    {
        public DbSet<Anime> AnimeList { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public MalContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
