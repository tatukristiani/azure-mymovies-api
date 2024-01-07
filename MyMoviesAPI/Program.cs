using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using MyMoviesAPI.Data;
using MyMoviesAPI.Data.Entities;
using MyMoviesAPI.Utility;
using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace MyMoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<ApplicationDbContext>();
            builder.Services.AddScoped<IDatabaseAgent,DatabaseAgent>();

            var SoftwareAllowOrigins = "_softwareAllowOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: SoftwareAllowOrigins,
                    policy =>
                    {
                        policy.WithOrigins(Environment.GetEnvironmentVariable("MYMOVIE_ALLOWED_ORIGINS").Split(";"))
                        .AllowAnyHeader()  // Allow any header, including Content-Type
                        .AllowAnyMethod(); // Allow any HTTP method
                    });
            });

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(SoftwareAllowOrigins);

            app.MapControllers();

            app.Run();
        }
    }
}