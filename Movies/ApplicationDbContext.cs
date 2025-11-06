using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movies.Models;
using Movies.ViewModel;

namespace Movies
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<Cinema> Directors { get; set; }
        public DbSet<ApplicationUserOTP> applicationUserOTPs { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Movies.ViewModel.ValidateOTPVM> ValidateOTPVM { get; set; } = default!;
        public DbSet<Movies.ViewModel.NewPasswordVM> NewPasswordVM { get; set; } = default!;
        public DbSet<Movies.ViewModel.ApplicationUserVM> ApplicationUserVM { get; set; } = default!;
    


    }
}
