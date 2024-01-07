using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Models.User
{
    public class UserPasswordUpdateDTO
    {
        public int ID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
