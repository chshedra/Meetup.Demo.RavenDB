using System.Text;
using System.Text.Json;
using Meetup.Demo.MessageBroker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.RavenDB.App;

public class Program
{
    private static void Main(string[] args)
    {
        var store = DocumentStoreHolder.Store;
        new Index_StockCountThingsGroupedIndex().Execute(store);

        //Consume session
        var sessionConsumer = new StockCountEventConsumer();
        sessionConsumer.StockCountEventReceived += SessionConsumer_StockCountSessionEventReceived;
        sessionConsumer.ConsumeStockCountMessage<StockCountSessionEvent>();

        //Consume batches
        var readBatchesConsumer = new StockCountEventConsumer();
        readBatchesConsumer.StockCountEventReceived +=
            ReadBatchesConsumer_StockCountEventReceivedAsync;
        readBatchesConsumer.ConsumeStockCountMessage<StockCountReadEvent>();

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();

        sessionConsumer.Dispose();
        readBatchesConsumer.Dispose();
    }

    private static void ReadBatchesConsumer_StockCountEventReceivedAsync(
        object? sender,
        BasicDeliverEventArgs e
    )
    {
        byte[] body = e.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var stockCountEvent = JsonSerializer.Deserialize<StockCountReadEvent>(json);

        var store = DocumentStoreHolder.Store;

        using var session = store.OpenSession();
        session.Store(stockCountEvent);
        Console.WriteLine($" [x] Batch {stockCountEvent?.BatchId}");
        session.SaveChanges();
    }

    private static void SessionConsumer_StockCountSessionEventReceived(
        object? sender,
        BasicDeliverEventArgs e
    )
    {
        byte[] body = e.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);
        var stockCountEvent = JsonSerializer.Deserialize<StockCountSessionEvent>(json);
        Console.WriteLine($" [x] {stockCountEvent?.SessionId}");
    }
}
