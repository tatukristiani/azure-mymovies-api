using Microsoft.AspNetCore.Http.HttpResults;
using MyMoviesAPI.Data.Entities;
using RestSharp;

namespace MyMoviesAPI.Data
{
    public class DatabaseAgent : IDatabaseAgent
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseAgent> _logger;

        public DatabaseAgent(ApplicationDbContext context, ILogger<DatabaseAgent> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ILogger<DatabaseAgent> Logger { get; }

        public bool AddEntity(object model)
        {
            _context.Add(model);
            return _context.SaveChanges() > 0;
        }

        public bool AddMovie(Movie movie)
        {
            _context.Movie.Add(movie);
            return _context.SaveChanges() > 0;
        }

        public IEnumerable<Movie> GetMovies()
        {
            return _context.Movie.ToList();
        }

        public IEnumerable<Movie> GetMoviesByUserId(int userId)
        {
            return _context.Movie
                .Where(movie => 
                    _context.UserMovie
                    .Where(userMovie => userMovie.User.ID == userId)
                    .Select(userMovie => userMovie.Movie.ID).ToList().Contains(movie.ID))
                .ToList();
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.User.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.User.Find(id);
        }

        public bool UserExists(string username, string email)
        {
            return (_context.User?.Any(e => e.Username == username || e.Email == email)).GetValueOrDefault();
        }

        public User? GetUserByUsername(string username)
        {
           return _context.User.FirstOrDefault(user => user.Username == username);
        }

        public User? GetUserByEmail(string email)
        {
            return _context.User.FirstOrDefault(user => user.Email == email);
        }

        public bool MovieExists(string movieTitle, string tmdbId)
        {
            return (_context.Movie?.Any(e => e.Title == movieTitle || e.TmdbId == tmdbId)).GetValueOrDefault();
        }

        public bool RemoveMovieFromUser(int movieTmdbId, int userId)
        {
            var movieFromDB = _context.Movie.Where(m => m.TmdbId == movieTmdbId.ToString()).FirstOrDefault();

            if (movieFromDB == null) return false;

            var userMovieResult = _context.UserMovie?.FirstOrDefault(userMovie => userMovie.User.ID == userId && userMovie.Movie.ID == movieFromDB.ID);

            if (userMovieResult == null) return false;

            _context.Remove(userMovieResult);
            return _context.SaveChanges() > 0;
        }

        public Movie? GetMovieById(int movieId)
        {
            return _context.Movie.Find(movieId);
        }

        public Movie? GetMovie(string title, string tmdbId)
        {
            return _context.Movie.FirstOrDefault(movie => movie.Title.Equals(title) && movie.TmdbId.Equals(tmdbId));
        }

        public bool UpdateUser(User user)
        {
            _context.User.Update(user);
            return _context.SaveChanges() > 0;
        }

        public List<PasswordResetRequest> GetPasswordResetRequests(string id)
        {
            return _context.PasswordResetRequest
                .Where(request => request.ID.Equals(id)).ToList();
        }
    }
}
