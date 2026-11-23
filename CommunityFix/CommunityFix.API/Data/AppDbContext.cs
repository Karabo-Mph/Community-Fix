using CommunityFix.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommunityFix.API.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // required for Identity tables

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Potholes" },
            new Category { Id = 2, Name = "Streetlights" },
            new Category { Id = 3, Name = "Waste & Illegal Dumping" },
            new Category { Id = 4, Name = "Water & Sanitation" }
        );

        modelBuilder.Entity<Report>()
            .HasIndex(r => r.ReferenceNumber)
            .IsUnique();
    }
}