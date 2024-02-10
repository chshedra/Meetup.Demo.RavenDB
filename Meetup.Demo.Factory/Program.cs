using Meetup.Demo.Domain;
using Meetup.Demo.Seeder;

internal class Program
{
    private static void Main(string[] args)
    {
        var store = DocumentStoreHolder.Store;

        using var session = store.OpenSession();
        using var dbContext = new AppDbContext();

        for (var i = 0; i < 500000; i++)
        {
            var product = new Product()
            {
                Id = $"ProductId-{i}",
                Name = $"Product {i}",
                Description = $"Description of product {i}"
            };

            session.Store(product);
            dbContext.Products.Add(product);
        }

        session.SaveChanges();
        dbContext.SaveChanges();
    }
}
