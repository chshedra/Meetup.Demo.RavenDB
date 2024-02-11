using Meetup.Demo.Domain;
using Meetup.Demo.MessageBroker;
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
            var producer = new EventProducer();

            var stockCountEvent = new StockCountSessionEvent()
            {
                SessionId = "Session1",
                StockCountId = "StockCountId",
            };

            producer.SendStockCountMessage(stockCountEvent);

            Console.WriteLine($" [x] Sent {stockCountEvent.SessionId}");
            Thread.Sleep(3000);

            //Send event batches
            var random = new Random();
            for (int i = 0; i < 6; i++)
            {
                var readEvent = new StockCountReadEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    StockCountId = "StocCount1",
                    SessionId = "Session1",
                    BatchId = $"Batch-{i}"
                };

                var things = new List<ThingReadInfo>();
                for (int j = 0; j < 10; j++)
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

                readEvent.ThingreadInfos = things;
                producer.SendStockCountMessage(readEvent);

                Console.WriteLine(
                    $" [x] Sent {readEvent.BatchId} {readEvent.ThingreadInfos.Count}"
                );
                Thread.Sleep(3000);
            }
        });

        Console.WriteLine(" Press [enter] to exit.");

        return Task.CompletedTask;
    }
}
