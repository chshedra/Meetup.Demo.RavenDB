using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Meetup.Demo.RavenDB;

public class DocumentStoreHolder
{
    private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

    public static IDocumentStore Store => store.Value;

    private static IDocumentStore CreateStore()
    {
        IDocumentStore store = new DocumentStore()
        {
            Urls = new[] { "https://a.free.shchedra.ravendb.cloud", },
            Conventions = { MaxNumberOfRequestsPerSession = 10, UseOptimisticConcurrency = true },
            Database = "test",
            Certificate = new X509Certificate2(
                "C:\\Users\\shedr\\source\\repos\\Meetup.Demo\\Meetup.Demo.RavenDB\\Meetup.Demo.RavenDB\\Cert\\free.shchedra.client.certificate.pfx"
            ),
        }.Initialize();

        CreateDataBaseIfNotExists(store);

        return store;
    }

    public static void CreateDataBaseIfNotExists(IDocumentStore store, string? database = null)
    {
        database ??= store.Database;

        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

        try
        {
            store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation());
        }
        catch (DatabaseDoesNotExistException)
        {
            store.Maintenance.Server.Send(
                new CreateDatabaseOperation(new DatabaseRecord(database))
            );
        }
    }
}
