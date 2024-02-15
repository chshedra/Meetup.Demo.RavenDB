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

                // using var session = store.OpenAsyncSession();

                //var ravenReadEvents = await session
                //    .Query<StockCountReadEvent>()
                //    .Where(x => x.Id != null)
                //    .Take(10)
                //    .ToListAsync();

                try
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    var postgresReadEvents1 = await _dbContext
                        .Things.Include(x => x.StockCount)
                        .Include(x => x.Product)
                        .Where(x => x.StockCount != null && x.StockCount.Id == "StockCountId-1")
                        .GroupBy(x => new { x.Product.Description, x.ZoneId })
                        .Select(x => new { x.Key.Description, x.Key.ZoneId })
                        .GroupBy(x => x.Description)
                        .Select(x => new { x.Key, Count = x.Sum(x => 1) })
                        .Where(x => x.Key.Contains("0"))
                        .OrderBy(x => x.Key)
                        .Skip(500)
                        .Take(100)
                        .ToListAsync();

                    sw.Stop();

                    Console.WriteLine(
                        $"Postgres things fetched: {postgresReadEvents1.Count} {sw.ElapsedMilliseconds}"
                    );

                    var sw1 = new Stopwatch();
                    sw1.Start();

                    var session = store.OpenAsyncSession();
                    var ravenReadEvents = await session
                        .Advanced.AsyncDocumentQuery<
                            StockCountGroupedResult,
                            Index_StockCountThingsGroupedIndex
                        >()
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
