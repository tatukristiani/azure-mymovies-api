using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyMoviesAPI.Data;
using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMovieController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<UserMovieController> _logger;

        public UserMovieController(IDatabaseAgent agent, ILogger<UserMovieController> logger)
        {
            _agent = agent;
            _logger = logger;
        }

        // GET api/UserMovie/User?id={id}
        [HttpGet("User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<IEnumerable<Movie>> GetUserMovies([FromQuery] int id)
        {
            try
            {
                var userMovies = _agent.GetMoviesByUserId(id);

                return userMovies != null ? Ok(userMovies) : NotFound(new List<Movie>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve users movies. Error: {ex}");
                return BadRequest($"Failed to retrieve users movies due to unknown reason.");
            }
        }
        // DELETE api/UserMovie?movieid={movieId}&userid={userId}
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult DeleteUserMovie([FromQuery]int movieTmdbId, [FromQuery]int userId)
        {
            try
            {
                bool movieRemoved = _agent.RemoveMovieFromUser(movieTmdbId, userId);

                return movieRemoved ? Ok("Movie successfully removed.") : BadRequest("Failed to remove movie from user.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to remove movie. Error: {ex}");
                return BadRequest($"Failed to remove movie due to unknown reason.");
            }
        }

        
        // POST /api/UserMovie?movieid={movieId}&userid={userId}
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult PostAddUserMovie([FromQuery] int movieId, [FromQuery] int userId)
        {
            try
            {
               var movie = _agent.GetMovieById(movieId);
               var user = _agent.GetUserById(userId);

                if (user != null && movie != null)
                {
                    UserMovie newUserMovie = new UserMovie()
                    {
                        User = user,
                        Movie = movie
                    };

                    return _agent.AddEntity(newUserMovie) ? Ok("Movie added to users movies.") : BadRequest("Failed to add movie to users movies.");
                }

                return BadRequest("Failed to add movie to users movies.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add movie to users movies. Error: {ex}");
                return BadRequest($"Failed to add movie to users movies due to unknown reason.");
            }
        }

        // POST /api/UserMovie/User?userid={userid}
        [HttpPost("User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult PostAddNewMovieForUser([FromQuery] int userId, [FromBody] Movie movie)
        {
            try
            {
                var movieExists = _agent.MovieExists(movie.Title, movie.TmdbId);
                var userFromDB = _agent.GetUserById(userId);

                if (userFromDB != null)
                {
                    if (!movieExists)
                    {
                        // Add the movie to database first
                        var movieAdded = _agent.AddMovie(movie);
                        var movieFromDB = _agent.GetMovie(movie.Title, movie.TmdbId);

                        if (movieFromDB != null)
                        { 
                            UserMovie newUserMovie = new UserMovie()
                            {
                                User = userFromDB,
                                Movie = movieFromDB
                            };

                            return _agent.AddEntity(newUserMovie) ? Ok("Movie added to users movies.") : BadRequest("Failed to add movie to users movies.");
                        }
                        return BadRequest("Failed to add movie to users records.");
                    }
                    else
                    {
                        var movieFromDB = _agent.GetMovie(movie.Title, movie.TmdbId);
                        if (movieFromDB != null)
                        {
                            // Add the movie and user ids to UserMovie table
                            UserMovie newUserMovie = new UserMovie()
                            {
                                User = userFromDB,
                                Movie = movie
                            };

                            return _agent.AddEntity(newUserMovie) ? Ok("Movie added to users movies.") : BadRequest("Failed to add movie to users movies.");
                        }
                        return BadRequest("Failed to add movie to users records.");
                    }
                }

                return BadRequest("Failed to add movie to users movies.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add movie to users movies. Error: {ex}");
                return BadRequest($"Failed to add movie to users movies due to unknown reason.");
            }
        }

    }
}
