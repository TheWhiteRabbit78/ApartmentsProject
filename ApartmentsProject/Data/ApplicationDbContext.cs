using ApartmentsProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add our apartment tables
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApartmentImage> ApartmentImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This keeps all the Identity tables

            // Configure decimal precision for price
            modelBuilder.Entity<Apartment>()
                .Property(a => a.Price)
                .HasPrecision(10, 2);

            // Configure relationship between Apartment and ApartmentImage
            modelBuilder.Entity<ApartmentImage>()
                .HasOne(img => img.Apartment)
                .WithMany(apt => apt.Images)
                .HasForeignKey(img => img.ApartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add some sample apartments (no images yet)
            modelBuilder.Entity<Apartment>().HasData(
                new Apartment
                {
                    Id = 1,
                    Title = "Stan A1 - 65m²",
                    Rooms = "2+1 (2 spavaće sobe)",
                    Description = "Prostran stan s modernim dizajnom, idealan za mlade obitelji.",
                    Price = 180000,
                    IsAvailable = true,
                    CreatedAt = new DateTime(2024, 1, 1) // Static date instead of DateTime.Now
                },
                new Apartment
                {
                    Id = 2,
                    Title = "Stan B2 - 78m²",
                    Rooms = "3+1 (3 spavaće sobe)",
                    Description = "Luksuzni stan s tri spavaće sobe, savršen za veće obitelji.",
                    Price = 220000,
                    IsAvailable = true,
                    CreatedAt = new DateTime(2024, 1, 1) // Static date instead of DateTime.Now
                }
            );
        }
    }
}