using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodDonationPlatform.Models;

namespace FoodDonationPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<FoodListing> FoodListings { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FoodListing>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FoodName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Unit).HasMaxLength(50).HasDefaultValue("servings");
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.Donor)
                    .WithMany(u => u.FoodListings)
                    .HasForeignKey(e => e.DonorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ExpirationDate);
                entity.HasIndex(e => e.DonorId);
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.PickupAddress).HasMaxLength(500);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FoodListing)
                    .WithMany(f => f.Orders)
                    .HasForeignKey(e => e.FoodListingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.FoodListingId);
                entity.HasIndex(e => e.Status);
            });
        }
    }
}
