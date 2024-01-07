namespace MyMoviesAPI.Data.Entities
{
    public class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public int Runtime { get; set; }
        public string TrailerId { get; set; }
        public string TmdbId { get; set; }
        public DateTime Date { get; set; }
        public string Genres { get; set; }
    }
}
