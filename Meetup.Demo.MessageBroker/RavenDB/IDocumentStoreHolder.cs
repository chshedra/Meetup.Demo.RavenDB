using Raven.Client.Documents;

namespace Meetup.Demo.Common.RavenDB;

public interface IDocumentStoreHolder
{
    IDocumentStore Store { get; }
}
