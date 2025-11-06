using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.Models;
using Movies.Repositories.IRepository;
using Movies.Repository;
using Movies.Utilities.DBInitilizer;
using System.IO;

namespace Movies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var ConnectionStrings = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.RegisterConfig(ConnectionStrings);

           
            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitilizer>();
            service!.Initialize();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapControllerRoute(
              name: "areas",          //Admin
              pattern: "{area:exists}/{controller=Category}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
              pattern: "{area=Identity}/{controller=Account}/{action=Register}/{id?}");
            app.MapControllerRoute(
              name: "areas",
              pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
            app.MapDefaultControllerRoute();


            app.Run();
        }
    }
}
