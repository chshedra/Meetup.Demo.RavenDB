namespace Meetup.Demo.Postgres.App;

using Meetup.Demo.Domain;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public DbSet<StockCountReadEvent> StockCountReadEvents { get; set; }

    public DbSet<ThingReadInfo> Things { get; set; }

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
