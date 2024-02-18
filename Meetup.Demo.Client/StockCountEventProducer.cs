using Meetup.Demo.Common.MessageBroker;
using Meetup.Demo.Domain;
using Microsoft.Extensions.Hosting;

namespace Meetup.Demo.Client;

public class StockCountEventProducer : BackgroundService
{
    public StockCountEventProducer() { }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            //send stock count session info
            Thread.Sleep(3000);

            var producer = new EventProducer();

            var stockCountEvent = new StockCountSessionEvent()
            {
                SessionId = "Session-1",
                StockCountId = "StockCountId-1",
                Status = "Active",
                StartedAt = DateTime.UtcNow,
            };

            producer.SendStockCountMessage(stockCountEvent);

            Console.WriteLine($" [x] Sent {stockCountEvent.SessionId}");
            Thread.Sleep(3000);

            //Send event batches
            var random = new Random();
            for (int i = 10; i < 100000; i++)
            {
                var readEvent = new StockCountReadEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    StockCountId = "StockCountId-1",
                    SessionId = "SessionId-1",
                    BatchId = $"Batch-{i}"
                };

                var things = new List<ThingReadInfo>();
                for (int j = 0; j < 100; j++)
                {
                    var zoneId = random.Next(1, 4);
                    var userId = random.Next(1, 10);
                    var thingReadInfo = new ThingReadInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = $"ProductId-{i}",
                        ThingId = $"ThingId-{i}{j}",
                        ZoneId = $"Zone-{zoneId}",
                        UserId = $"User-{userId}"
                    };
                    things.Add(thingReadInfo);
                }

                readEvent.Things = things;
                producer.SendStockCountMessage(readEvent);

                Console.WriteLine($" [x] Sent {readEvent.BatchId} {readEvent.Things.Count}");
                Thread.Sleep(1000);
            }
        });

        Console.WriteLine(" Press [enter] to exit.");

        return Task.CompletedTask;
    }
}
