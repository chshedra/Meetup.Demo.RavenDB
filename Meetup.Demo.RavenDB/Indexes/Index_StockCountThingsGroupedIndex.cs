using Meetup.Demo.Domain;
using Raven.Client.Documents.Indexes;

namespace Meetup.Demo.RavenDB.App.Indexes;

public class Index_StockCountThingsGroupedIndex
    : AbstractIndexCreationTask<Thing, StockCountGroupedResult>
{
    public override string IndexName => "StockCountThingsGrouped";

    public Index_StockCountThingsGroupedIndex()
    {
        Map = things =>
            from thing in things
            let desc = LoadDocument<Product>(thing.ProductId, "Products").Description
            select new StockCountGroupedResult
            {
                ProductId = thing.ProductId,
                Description = desc,
                ZoneCounts = new(),
                Count = 1
            };

        Reduce = results =>
            from result in results
            group result by result.ProductId into g
            select new
            {
                ProductId = g.Key,
                Description = g.Select(x => x.Description).FirstOrDefault(),
                ZoneCounts = g.GroupBy(x => x.ZoneId)
                    .ToDictionary(x => x.Key, x => x.Sum(x => x.Count)),
                Count = g.Sum(x => x.Count)
            };
    }
}
