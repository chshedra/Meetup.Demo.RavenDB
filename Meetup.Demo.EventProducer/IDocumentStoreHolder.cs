using Raven.Client.Documents;

namespace Meetup.Demo.Client;

public interface IDocumentStoreHolder
{
    IDocumentStore Store { get; }
}
