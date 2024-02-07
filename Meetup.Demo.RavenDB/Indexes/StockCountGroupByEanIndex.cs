using Meetup.Demo.RavenDB.Domain;
using Raven.Client.Documents.Indexes;

namespace Meetup.Demo.RavenDB.App.Indexes;

public class Products_ByCategory : AbstractIndexCreationTask<Asset, string>
{
    public Products_ByCategory()
    {
        OutputReduceToCollection = "GroupedEans";
        Map = assets => assets.Select(x => x.Name);
        Reduce = results => results.GroupBy(x => x);
    }
}