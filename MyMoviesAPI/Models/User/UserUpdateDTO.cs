using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Models.User
{
    public class UserUpdateDTO
    {
        public int ID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }

        public bool IsUserValid()
        {
            if (!IsUsernameValid() && IsEmailValid()) return true; // Only Email with value -> OK
            if (!IsEmailValid() && IsUsernameValid()) return true; // Only Username with value -> OK
            if (IsUsernameValid() && IsEmailValid()) return true; // Both Email and Username with value -> OK
            return false;
        }

        public bool IsUsernameValid()
        {
            return !String.IsNullOrWhiteSpace(Username);
        }

        public bool IsEmailValid()
        {
            return !String.IsNullOrWhiteSpace(Email);
        }
    }
}
