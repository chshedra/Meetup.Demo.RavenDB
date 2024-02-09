namespace Meetup.Demo.Postgres.App;

using Meetup.Demo.RavenDB.Domain;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<StockCountReadEvent> StockCountReadEvents { get; set; }

    public AppDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=пароль_от_postgres"
        );
    }
}
