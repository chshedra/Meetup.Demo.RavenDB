using Meetup.Demo.Postgres.App;
using Meetup.Demo.RavenDB.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    private static void Main(string[] args)
    {
        using (var dbContext = new AppDbContext())
        {
            var product = new Product()
            {
                Id = "Product-1",
                Name = "Product 1",
                Description = "Description of product 1"
            };

            dbContext.Add(product);
            dbContext.SaveChanges();
        }

        using (var dbContext = new AppDbContext())
        {
            var product = dbContext.Products.First();

            Console.WriteLine($"{product.Id}");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(
                (hostContext, services) =>
                {
                    services.AddScoped<DbContext, AppDbContext>();
                }
            );
}
