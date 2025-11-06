using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Repository;
using Movies.Utilities;
using Movies.Utilities.DBInitilizer;

namespace Movies
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services, string connection)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                //option.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                option.UseSqlServer(connection);
            });

          
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            services.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();

            services.AddScoped<IDBInitilizer, DBInitilizer>();



        }
    }
}
