using System.Text;
using System.Text.Json;
using Meetup.Demo.Common.MessageBroker;
using Meetup.Demo.Common.RavenDB;
using Meetup.Demo.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.RavenDB.App;

public class StockCountReadEventConsumer : EventConsumerBase
{
    private readonly IDocumentStoreHolder _documentStoreHolder;

    public StockCountReadEventConsumer(IDocumentStoreHolder documentStoreHolder)
        : base(nameof(StockCountReadEvent))
    {
        _documentStoreHolder = documentStoreHolder;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, e) =>
        {
            byte[] body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var stockCountEvent = JsonSerializer.Deserialize<StockCountReadEvent>(json);

            if (stockCountEvent != null)
            {
                var things = stockCountEvent.Things.Select(x => new Thing
                {
                    Id = x.ThingId,
                    ProductId = x.ProductId,
                    ZoneId = x.ZoneId
                });

                using var session = _documentStoreHolder.Store.OpenAsyncSession();
                await session.StoreAsync(things);
                await session.SaveChangesAsync();
            }

            Console.WriteLine($" [x] Batch {stockCountEvent?.BatchId}");
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
