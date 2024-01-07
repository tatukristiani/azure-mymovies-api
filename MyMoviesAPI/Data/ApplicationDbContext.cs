using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMoviesAPI.Data.Entities;

namespace MyMoviesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _config;
        public ApplicationDbContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserMovie> UserMovie { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder bldr)
        {
            base.OnConfiguring(bldr);

            bldr.UseSqlServer(_config.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"));
        }
    }
}
