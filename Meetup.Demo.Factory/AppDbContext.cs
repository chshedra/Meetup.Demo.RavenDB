namespace Meetup.Demo.Seeder;

using Meetup.Demo.RavenDB.Domain;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<StockCountReadEvent> StockCountReadEvents { get; set; }

    public AppDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=meetu-demo;Username=postgres;Password=postgres"
        );
    }
}
