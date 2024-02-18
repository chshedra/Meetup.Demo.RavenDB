using Meetup.Demo.Common.Postgres;
using Meetup.Demo.Common.RavenDB;
using Meetup.Demo.Domain;

internal class Program
{
    private static void Main(string[] args)
    {
        var store = new DocumentStoreHolder().Store;

        // using var session = store.OpenSession();
        using var dbContext = new AppDbContext();

        for (var i = 56878; i < 500000; i++)
        {
            var product = new Product()
            {
                Id = $"ProductId-{i}",
                Name = $"Product {i}",
                Description = $"Description of product {i}"
            };

            //session.Store(product);
            dbContext.Products.Add(product);

            if (i % 10 == 0)
            {
                dbContext.SaveChanges();
            }
        }
        //for (int i = 0; i < 10; i++)
        //{
        //    var random = new Random();
        //    var things = new List<Thing>();
        //    for (int j = 0; j < 10; j++)
        //    {
        //        var zoneId = random.Next(1, 4);
        //        var userId = random.Next(1, 10);
        //        var thingReadInfo = new Thing()
        //        {
        //            ProductId = $"ProductId-{i}",
        //            Id = $"ThingId-{i}{j}",
        //            ZoneId = $"Zone-{zoneId}",
        //            StockCountId = "StockCountId-1"
        //        };
        //        things.Add(thingReadInfo);
        //    }
        //    dbContext.Things.AddRange(things);

        //    //session.SaveChanges();
        //}
    }
}
