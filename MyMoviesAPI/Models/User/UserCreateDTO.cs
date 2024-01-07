using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Models.User
{
    public class UserCreateDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
