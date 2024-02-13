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
            for (int i = 0; i < 6; i++)
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

                    var postgresReadEvents = await _dbContext
                        .Things.Include(x => x.StockCount)
                        .Include(x => x.Product)
                        .Where(x => x.StockCount != null && x.StockCount.Id == "StockCountId-1")
                        .GroupBy(x => x.ProductId)
                        .Select(x => new StockCountGroupedResult
                        {
                            ProductId = x.Key,
                            Description = x.First().Product.Description,
                            ZoneCounts = x.GroupBy(x => x.ZoneId)
                                .ToDictionary(x => x.Key, x => x.Sum(x => 1)),
                            Count = x.Sum(x => 1)
                        })
                        .ToListAsync();

                    sw.Stop();

                    Console.WriteLine(
                        $"Events fetched: {postgresReadEvents.Count} {sw.ElapsedMilliseconds}"
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
