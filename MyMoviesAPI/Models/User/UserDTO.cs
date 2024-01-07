using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Models.User
{
    public class UserDTO
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
