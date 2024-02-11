using Meetup.Demo.Domain;
using Raven.Client.Documents.Indexes;

namespace Meetup.Demo.RavenDB.App.Indexes;

public class Index_StockCountThingsGroupedIndex
    : AbstractIndexCreationTask<Thing, Index_StockCountThingsGroupedIndex.Result>
{
    public class Result
    {
        public string ProductId { get; set; }

        public string ZoneId { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }
    }

    public override string IndexName => "StockCountThingsGrouped";

    public Index_StockCountThingsGroupedIndex()
    {
        Map = things =>
            from thing in things
            let desc = LoadDocument<Product>(thing.ProductId, "Products").Description
            select new Result
            {
                ProductId = thing.ProductId,
                ZoneId = thing.ZoneId,
                Description = desc,
                Count = 1
            };

        Reduce = results =>
            from result in results
            group result by result.ProductId into g
            select new
            {
                ProductId = g.Key,
                ZoneId = g.Select(x => x.ZoneId).Distinct(),
                Description = g.Select(x => x.Description).FirstOrDefault(),
                Count = g.Sum(x => x.Count)
            };
    }
}
