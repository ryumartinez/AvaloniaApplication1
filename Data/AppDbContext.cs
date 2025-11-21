using AvaloniaApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication1.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    // Configures the database to use a local file named "app.db"
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    // Seeds some initial data for visualization
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 50 },
            new Product { Id = 2, Name = "Mouse", Price = 25.50m, Stock = 200 },
            new Product { Id = 3, Name = "Keyboard", Price = 45.00m, Stock = 150 },
            new Product { Id = 4, Name = "Monitor", Price = 199.99m, Stock = 75 }
        );
    }
}