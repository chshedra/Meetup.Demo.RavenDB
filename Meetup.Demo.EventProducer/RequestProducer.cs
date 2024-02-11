using Meetup.Demo.Domain;
using Meetup.Demo.Postgres.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Meetup.Demo.Client;

public class RequestProducer : BackgroundService
{
    private readonly IDocumentStoreHolder _documentStoreHolder;

    public RequestProducer(IDocumentStoreHolder documentStoreHolder)
    {
        _documentStoreHolder = documentStoreHolder;
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
                using var dbContext = new AppDbContext();

                //var ravenReadEvents = await session
                //    .Query<StockCountReadEvent>()
                //    .Where(x => x.Id != null)
                //    .Take(10)
                //    .ToListAsync();

                try
                {
                    var postgresReadEvents = await dbContext
                        .Things.Include(x => x.StockCountReadEvent)
                        .Where(x =>
                            x.StockCountReadEvent != null
                            && x.StockCountReadEvent.SessionId == "Session1"
                        )
                        .GroupBy(x => x.ProductId)
                        .ToListAsync();
                    Console.WriteLine($"Events fetched: {postgresReadEvents.Count}");
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
