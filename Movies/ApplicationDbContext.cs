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
        public DbSet<Cart> carts { get; set; } 
        public DbSet<Promotion> promotions { get; set; } 
        public DbSet<ApplicationUserOTP> applicationUserOTPs { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ValidateOTPVM> ValidateOTPVM { get; set; } = default!;
        public DbSet<NewPasswordVM> NewPasswordVM { get; set; } = default!;
        public DbSet<ApplicationUserVM> ApplicationUserVM { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key
            modelBuilder.Entity<Cart>()
                .HasKey(c => new { c.ApplicationUserId, c.Mov_Id });

            // Relation: Cart → Movie
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Movie)
                .WithMany()
                .HasForeignKey(c => c.Mov_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Cart → User
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
