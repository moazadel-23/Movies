using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Models;

namespace ECommerce.DataAccess.EntityConfigurations
{
    public class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            // Composite Key
            builder.HasKey(e => new { e.Mov_Id, e.ApplicationUserId });

            // Cart → Movie
            builder.HasOne(e => e.Movie)
                   .WithMany()
                   .HasForeignKey(e => e.Mov_Id)
                   .OnDelete(DeleteBehavior.Cascade);

            // Cart → User
            builder.HasOne(e => e.User)
                   .WithMany()
                   .HasForeignKey(e => e.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
