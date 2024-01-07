using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyMoviesAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text.Json;

namespace MyMoviesAPI.Data
{
    public static class HttpMovieAgent
    {
        private static string ACCESS_TOKEN = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhYzRlYWQyYmJkZTQ5ZjNjYjM0MjQxM2MwOWY2ZDI1YSIsInN1YiI6IjYyMDBkZDRhN2ZjYWIzMDA2YzUyYzljMCIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.CvpVoF7DpV7QVU76mHP41NSbTbSUyplSc5ijm56ffeE";

        public static async Task<object?> HttpFetchTrendingMovies(int page)
        {
            return await SendHttpRequest($"https://api.themoviedb.org/3/trending/movie/week?language=en-US&page={page}");  
        }

        public static async Task<object?> HttpFetchTopRatedMovies(int page)
        {
            return await SendHttpRequest($"https://api.themoviedb.org/3/movie/top_rated?language=en-US&page={page}");
        }

        public static async Task<object?> HttpFetchHorrorMovies(int page)
        {
            var genreCode = FetchGenreCode("Horror");
            return await SendHttpRequest($"https://api.themoviedb.org/3/discover/movie?with_genres={genreCode}&page={page}");
        }

        public static async Task<object?> HttpFetchComedyMovies(int page)
        {
            var genreCode = FetchGenreCode("Comedy");
            return await SendHttpRequest($"https://api.themoviedb.org/3/discover/movie?with_genres={genreCode}&page={page}");
        }

        public static async Task<object?> HttpFetchRomanceMovies(int page)
        {
            var genreCode = FetchGenreCode("Romance");
            return await SendHttpRequest($"https://api.themoviedb.org/3/discover/movie?with_genres={genreCode}&page={page}");
        }

        public static async Task<object?> HttpFetchActionMovies(int page)
        {
            var genreCode = FetchGenreCode("Action");
            return await SendHttpRequest($"https://api.themoviedb.org/3/discover/movie?with_genres={genreCode}&page={page}");
        }

        public static async Task<object?> HttpFetchDocumentaryMovies(int page)
        {
            var genreCode = FetchGenreCode("Documentary");
            return await SendHttpRequest($"https://api.themoviedb.org/3/discover/movie?with_genres={genreCode}&page={page}");
        }

        public static async Task<object?> HttpSearchMovie(string movie)
        {
            return await SendHttpRequest($"https://api.themoviedb.org/3/search/movie?query={movie}");
        }

        public static int? FetchGenreCode(string genre)
        {
            var options = new RestClientOptions("https://api.themoviedb.org/3/genre/movie/list?language=en");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", ACCESS_TOKEN);
            var response = client.GetAsync(request);

            return System.Text.Json.JsonSerializer.Deserialize<GenresData>(response.Result.Content)?.genres?.FirstOrDefault(g => g.name == genre)?.id;
        }

        private async static Task<IEnumerable<MovieDTO>?> SendHttpRequest(string url)
        {
            var options = new RestClientOptions(url);
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", ACCESS_TOKEN);
            var response = await client.GetAsync(request);

            return JsonConvert.DeserializeObject<IEnumerable<MovieDTO>>(JObject.Parse(response.Content)["results"].ToString());
        }
    }
}
