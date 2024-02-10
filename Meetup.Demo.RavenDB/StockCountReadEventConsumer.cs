using System.Text;
using System.Text.Json;
using Meetup.Demo.Domain;
using Meetup.Demo.MessageBroker;
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
        consumer.Received += (model, e) =>
        {
            byte[] body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var stockCountEvent = JsonSerializer.Deserialize<StockCountReadEvent>(json);

            using var session = _documentStoreHolder.Store.OpenSession();
            session.Store(stockCountEvent);
            Console.WriteLine($" [x] Batch {stockCountEvent?.BatchId}");
            session.SaveChanges();
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
