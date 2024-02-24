using Meetup.Demo.Domain;
using Raven.Client.Documents.Indexes;

namespace Meetup.Demo.Common.RavenDB;

public class Index_StockCountThingsGroupedIndex
    : AbstractIndexCreationTask<Thing, StockCountGroupedResult>
{
    public override string IndexName => "StockCountThingsGrouped";

    public override IndexDefinition CreateIndexDefinition()
    {
        return new IndexDefinition
        {
            Maps =
            {
                @"from thing in docs.Things  
                    let product = LoadDocument(thing.ProductId, ""Products"")
                     select new
                    {
                        thing.ProductId,
                        ProductName = product.Name,
                        ProductDescription = product.Description,
                        ZoneCounts = thing.ZoneId,
                        Counted = 1
                    }"
            },
            Reduce =
                @"from result in results
                    group result by  result.ProductName into g
                    select new 
                    {
                        ProductName = g.Key,
                        ProductId = g.First().ProductId,
                        ProductDescription = g.First().ProductDescription,
                        ZoneCounts = g.GroupBy(x => x.ZoneCounts)
                            .ToDictionary(x => x.Key, x => x.Sum(x => x.Counted)),
                        Counted = g.Sum(x => x.Counted)
                    }"
        };
    }
}
