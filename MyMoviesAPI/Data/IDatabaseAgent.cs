using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Data
{
    public interface IDatabaseAgent
    {
        IEnumerable<User> GetUsers();
        IEnumerable<Movie> GetMovies();
        IEnumerable<Movie> GetMoviesByUserId(int userId);
        Movie? GetMovieById(int movieId);
        Movie? GetMovie(string title, string tmdbId);
        User? GetUserById(int userId);
        User? GetUserByUsername(string username);
        User? GetUserByEmail(string email);
        bool UserExists(string username, string email);
        bool MovieExists(string movieTitle, string tmdbId);
        bool RemoveMovieFromUser(int movieId, int userId);
        List<PasswordResetRequest> GetPasswordResetRequests(string id);
        bool AddEntity(object model);
        bool AddMovie(Movie movie);
        bool UpdateUser(User user);
    }
}
