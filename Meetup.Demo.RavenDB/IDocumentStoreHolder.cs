using Raven.Client.Documents;

namespace Meetup.Demo.RavenDB.App;

public interface IDocumentStoreHolder
{
    IDocumentStore Store { get; }
}
