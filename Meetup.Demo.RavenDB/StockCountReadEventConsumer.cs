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

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, e) =>
        {
            byte[] body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var stockCountEvent = JsonSerializer.Deserialize<StockCountReadEvent>(json);

            if (stockCountEvent != null)
            {
                try
                {
                    using var session = _documentStoreHolder.Store.OpenAsyncSession();

                    foreach (var thingInfo in stockCountEvent.Things)
                    {
                        var thing = new Thing
                        {
                            Id = thingInfo.ThingId,
                            ProductId = thingInfo.ProductId,
                            ZoneId = thingInfo.ZoneId,
                            StockCountId = stockCountEvent.StockCountId
                        };
                        await session.StoreAsync(thing);
                    }

                    await session.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine($" [x] Batch {stockCountEvent?.BatchId}");
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
