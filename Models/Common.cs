namespace Models
{
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
        public string? heatMapUri { get; set; }
        public List<Anime> anime { get; set; } = new();
    }
}
