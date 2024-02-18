using System.Diagnostics;
using Meetup.Demo.Common.Postgres;
using Meetup.Demo.Common.RavenDB;
using Meetup.Demo.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Meetup.Demo.Client;

public class RequestProducer : BackgroundService
{
    private readonly IDocumentStoreHolder _documentStoreHolder;
    private readonly AppDbContext _dbContext;

    public RequestProducer(IDocumentStoreHolder documentStoreHolder, AppDbContext dbContext)
    {
        _documentStoreHolder = documentStoreHolder;
        _dbContext = dbContext;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(async () =>
        {
            for (int i = 0; i < 3000; i++)
            {
                Thread.Sleep(3000);
                var store = _documentStoreHolder.Store;

                try
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    var postgresReadEvents = await _dbContext
                        .Things.Include(x => x.StockCount)
                        .Include(x => x.Product)
                        .GroupBy(x => x.Product.Name)
                        .Select(x => new
                        {
                            ProductName = x.Key,
                            x.First().Product.Description,
                            Zones = x.GroupBy(x => x.ZoneId)
                                .Select(x => new { x.Key, Counted = x.Count() }),
                            Counted = x.Count()
                        })
                        .Skip(500)
                        .Take(100)
                        .ToListAsync();

                    sw.Stop();

                    Console.WriteLine(
                        $"Postgres things fetched: {postgresReadEvents.Count} {sw.ElapsedMilliseconds}"
                    );

                    var sw1 = new Stopwatch();
                    sw1.Start();

                    var session = store.OpenAsyncSession();
                    var ravenReadEvents = await session
                        .Advanced.AsyncDocumentQuery<
                            StockCountGroupedResult,
                            Index_StockCountThingsGroupedIndex
                        >()
                        .OrderBy(x => x.ProductId)
                        .Skip(500)
                        .Take(100)
                        .ToListAsync();

                    sw1.Stop();

                    Console.WriteLine(
                        $"Raven things fetched: {ravenReadEvents.Count} {sw1.ElapsedMilliseconds}"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        });

        return Task.CompletedTask;
    }
}
