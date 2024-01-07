using MyMoviesAPI.Data.Entities;
using Newtonsoft.Json;

namespace MyMoviesAPI.Models
{
    public class MovieDTO
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("genre_ids")]
        public List<int> Genres { get; set; }
    }
}
