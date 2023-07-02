namespace Models
{
    public class ResponseModel
    {
        public Datum[] data { get; set; } = null!;
        public Paging paging { get; set; } = null!;
    }

    public class Paging
    {
        public string? previous { get; set; }
        public string? next { get; set; }
    }

    public class Datum
    {
        public Anime node { get; set; } = null!;
    }
}
