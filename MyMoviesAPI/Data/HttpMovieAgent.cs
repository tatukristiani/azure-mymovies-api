using AutoMapper.Configuration.Conventions;
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
        private static string ACCESS_TOKEN = Environment.GetEnvironmentVariable("ACCESS_TOKEN_TMDB");

        // MAX Page number: 500
        public static async Task<object?> HttpFetchTrendingMovies(int page)
        {
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/trending/movie/week?language=en-US&page=${page}", null);
        }

        // MAX Page number: 454 (Not yet used)
        public static async Task<object?> HttpFetchTopRatedMovies(int page)
        {
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/movie/top_rated?language=en-US&page=${page}", null);
        }

        // MAX Page number: 500
        public static async Task<object?> HttpFetchHorrorMovies(int page)
        {
            var genreCode = FetchGenreCode("Horror");
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/discover/movie?with_genres=${genreCode}&page=${page}", genreCode);
        }

        // MAX Page number: 500
        public static async Task<object?> HttpFetchComedyMovies(int page)
        {
            var genreCode = FetchGenreCode("Comedy");
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/discover/movie?with_genres=${genreCode}&page=${page}", genreCode);
        }

        // MAX Page number: 500
        public static async Task<object?> HttpFetchRomanceMovies(int page)
        {
            var genreCode = FetchGenreCode("Romance");
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/discover/movie?with_genres=${genreCode}&page=${page}", genreCode);
        }

        // MAX Page number: 500
        public static async Task<object?> HttpFetchActionMovies(int page)
        {
            var genreCode = FetchGenreCode("Action");
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/discover/movie?with_genres=${genreCode}&page=${page}", genreCode);
        }

        // MAX Page number: 500
        public static async Task<object?> HttpFetchDocumentaryMovies(int page)
        {
            var genreCode = FetchGenreCode("Documentary");
            return await HttpFetchMultiplePagesOfMovies(page, "https://api.themoviedb.org/3/discover/movie?with_genres=${genreCode}&page=${page}", genreCode);
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

        // Retrieves movies from 5 different pages
        // Last page to be retrieved from TMDB: page * 4 + page
        // First page: last page - 4
        private async static Task<IEnumerable<MovieDTO>?> HttpFetchMultiplePagesOfMovies(int page, string url, int? genreCode)
        {
            List<string> urls = new List<string>();
            int endPage = page * 4;
            int startPage = endPage - 3;
            List<MovieDTO>? movies = new List<MovieDTO>();

            for (int currentPage = startPage; currentPage <= endPage; currentPage++)
            {
                urls.Add(url.Replace("${page}", currentPage.ToString()).Replace("${genreCode}", genreCode.ToString()));
            }

            var requests = urls.Select(url => SendHttpRequest(url)).ToList();

            await Task.WhenAll(requests);

            var responses = requests.Select(task => task.Result);

            foreach (var response in responses)
            {
                if (response != null) movies.AddRange(response);
            }

            return movies;
        }
    }
}
