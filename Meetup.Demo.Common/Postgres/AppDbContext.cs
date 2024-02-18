using Meetup.Demo.Domain;
using Microsoft.EntityFrameworkCore;

namespace Meetup.Demo.Common.Postgres;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public DbSet<StockCount> StockCounts { get; set; }

    public DbSet<Thing> Things { get; set; }

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
            "Host=localhost;Port=5432;Database=meetup-demo;Username=postgres;Password=postgres"
        );
    }
}
