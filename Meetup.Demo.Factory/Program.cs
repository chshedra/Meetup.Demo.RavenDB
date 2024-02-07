using Meetup.Demo.Factory;
using Meetup.Demo.RavenDB.Domain;

var store = DocumentStoreHolder.Store;

using (var session = store.OpenSession())
{
    for (var i = 100; i < 500000; i++)
    {
        session.Store(
            new Product()
            {
                Id = $"ProductId-{i}",
                Name = $"Product {i}",
                Description = $"Description of product {i}"
            }
        );
    }

    session.SaveChanges();
}
