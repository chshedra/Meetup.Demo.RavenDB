using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Meetup.Demo.Client;

public class DocumentStoreHolder : IDocumentStoreHolder
{
    public IDocumentStore Store { get; }

    public DocumentStoreHolder()
    {
        var store = new DocumentStore
        {
            Urls = new[] { "https://a.ashchedra.ravendb.community:8080/", },
            Conventions = { MaxNumberOfRequestsPerSession = 10, UseOptimisticConcurrency = true },
            Database = "test",
            Certificate = new X509Certificate2(
                "D:\\RavenDB\\ashchedra.Cluster.Settings 2024-02-07 16-02\\admin.client.certificate.ashchedra.pfx"
            ),
        }.Initialize();

        store.OnBeforeQuery += (sender, beforeQueryExecutedArgs) =>
        {
            beforeQueryExecutedArgs.QueryCustomization.WaitForNonStaleResults();
        };

        CreateDataBaseIfNotExists(store);

        Store = store;
    }

    private void CreateDataBaseIfNotExists(IDocumentStore store, string? database = null)
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
