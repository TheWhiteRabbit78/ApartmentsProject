using ApartmentsProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext(options)
{
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<ApartmentImage> ApartmentImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Apartment>()
            .Property(a => a.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ApartmentImage>()
            .HasOne(img => img.Apartment)
            .WithMany(apt => apt.Images)
            .HasForeignKey(img => img.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}