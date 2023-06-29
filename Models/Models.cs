using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class MalContext : DbContext
    {
        public DbSet<Anime> AnimeList { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public MalContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name });
        }
    }

    public class ResponseModel
    {
        public Datum[] data { get; set; } = null!;
        public Paging paging { get; set; } = null!;
        public Season season { get; set; } = null!;
    }

    public class Paging
    {
        public string next { get; set; } = null!;
    }

    public class Season
    {
        public int year { get; set; }
        public string season { get; set; } = null!;
    }

    public class Datum
    {
        public Anime node { get; set; } = null!;
    }

    public class Anime
    {
        public int id { get; set; }
        public string title { get; set; } = null!;
        public float? mean { get; set; }
        public List<Genre> genres { get; set; } = new();
    }

    public class Genre
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public float? mean { get; set; }
        public float? median { get; set; }
        public string? heatMapFileName { get; set; }
        public List<Anime> anime { get; set; } = new();
    }

    public class GenreComparer : Comparer<Genre>
    {
        public override int Compare(Genre? x, Genre? y)
        {
            if(x == null && y == null) return 0;
            if(x == null) return -1;
            if(y == null) return 1;
            return x.id.CompareTo(y.id);
        }
    }

    public class GenreEqualityComparer : EqualityComparer<Genre>
    {
        public override bool Equals(Genre? x, Genre? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.id == y.id && x.name == y.name;
        }

        public override int GetHashCode(Genre obj)
        {
            int hashId = obj.id.GetHashCode();
            int hashName = obj.name.GetHashCode();

            return hashId ^ hashName;
        }
    }
}
