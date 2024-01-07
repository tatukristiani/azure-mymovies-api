using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MyMoviesAPI.Data.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserType Type { get; set; }
        public DateTime AccountCreated { get; set; }
        public UserStatus Status { get; set; }
    }
}
