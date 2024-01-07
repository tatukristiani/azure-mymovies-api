namespace MyMoviesAPI.Data.Entities
{
    public class UserMovie
    {
        public int ID { get; set; } 
        public User User { get; set; }
        public Movie Movie { get; set; }
    }
}
