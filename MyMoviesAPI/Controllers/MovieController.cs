using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MyMoviesAPI.Data;
using MyMoviesAPI.Data.Entities;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyMoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MovieController : ControllerBase
    {
        private readonly IDatabaseAgent _agent;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IDatabaseAgent agent, ILogger<MovieController> logger)
        {
            _agent = agent;
            _logger = logger;
        }


        // POST api/Movie
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public ActionResult Post([FromBody] Movie movie)
        {
            try
            {
                if (_agent.MovieExists(movie.Title, movie.TmdbId))
                {
                    return BadRequest($"Failed to add movie. Movie already exists.");
                }
                
                bool movieCreated = _agent.AddEntity(movie);

                return movieCreated ? Ok("Movie added.") : BadRequest("Failed to add movie.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to add movie. Error: {ex}");
                return BadRequest($"Failed to add movie due to an unknown reason.");
            }
        }

        [HttpGet]
        [Route("trending")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetTrendingMovies([FromQuery] int page)
        {
            try
            {
                var result = await HttpMovieAgent.HttpFetchTrendingMovies(page);
                return Ok(result);
            }
            catch(Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("toprated")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetTopRated([FromQuery] int page)
        {

            try
            {
                var result = await HttpMovieAgent.HttpFetchTopRatedMovies(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("horror")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetHorrorMovies([FromQuery] int page)
        {
            try
            {
                var result = await HttpMovieAgent.HttpFetchHorrorMovies(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("action")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetActionMovies([FromQuery] int page)
        {
            try
            {
                var result = await HttpMovieAgent.HttpFetchActionMovies(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("comedy")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetComedyMovies([FromQuery] int page)
        {
            try
            {
                var result = await HttpMovieAgent.HttpFetchComedyMovies(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("documentary")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetDocumentaryMovies([FromQuery] int page)
        {
            try
            {
                var result = await HttpMovieAgent.HttpFetchDocumentaryMovies(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("romance")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> GetRomanceMovies([FromQuery] int page)
        {
            try
            {
                var result = await HttpMovieAgent.HttpFetchRomanceMovies(page);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> SearchMovie([FromQuery] string movie)
        {
            try
            {
                var result = await HttpMovieAgent.HttpSearchMovie(movie);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Could not fetch movies.");
            }
        }
    }
}
