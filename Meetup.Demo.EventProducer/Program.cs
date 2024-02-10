using Meetup.Demo.MessageBroker;
using Meetup.Demo.RavenDB.Domain;

internal class Program
{
    private static void Main(string[] args)
    {
        //send stock count session info
        Thread.Sleep(5000);
        var producer = new StockCountEventProducer();

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
        for (int i = 0; i < 10; i++)
        {
            var readEvent = new StockCountReadEvent()
            {
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
                    ProductId = $"ProductId-{i}",
                    ThingId = $"ThingId-{i}{j}",
                    ZoneId = $"Zone-{zoneId}",
                    UserId = $"User-{userId}"
                };
                things.Add(thingReadInfo);
            }

            readEvent.ThingsBatch = things;

            producer.SendStockCountMessage(readEvent);
            Console.WriteLine($" [x] Sent {readEvent.BatchId} {readEvent.ThingsBatch.Count}");
            Thread.Sleep(100);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
