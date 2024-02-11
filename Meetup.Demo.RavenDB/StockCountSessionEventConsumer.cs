using System.Text;
using System.Text.Json;
using Meetup.Demo.Common.MessageBroker;
using Meetup.Demo.Common.RavenDB;
using Meetup.Demo.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.RavenDB.App;

public class StockCountSessionEventConsumer : EventConsumerBase
{
    private readonly IDocumentStoreHolder _storeHolder;

    public StockCountSessionEventConsumer(IDocumentStoreHolder documentStoreHolder)
        : base(nameof(StockCountSessionEvent))
    {
        _storeHolder = documentStoreHolder;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, e) =>
        {
            byte[] body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var stockCountEvent = JsonSerializer.Deserialize<StockCountSessionEvent>(json);

            if (stockCountEvent != null)
            {
                var store = _storeHolder.Store;
                using var session = store.OpenAsyncSession();

                var stockCount = new StockCount
                {
                    Id = stockCountEvent.StockCountId,
                    StartedAt = stockCountEvent.StartedAt,
                    Status = stockCountEvent.Status
                };

                await session.StoreAsync(stockCount);
                await session.SaveChangesAsync();
            }

            Console.WriteLine($" [x] {stockCountEvent?.SessionId}");
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
