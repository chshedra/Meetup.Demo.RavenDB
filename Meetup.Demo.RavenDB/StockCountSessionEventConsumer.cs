using System.Text;
using System.Text.Json;
using Meetup.Demo.Common.MessageBroker;
using Meetup.Demo.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.RavenDB.App;

public class StockCountSessionEventConsumer : EventConsumerBase
{
    public StockCountSessionEventConsumer()
        : base(nameof(StockCountSessionEvent)) { }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, e) =>
        {
            byte[] body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var stockCountEvent = JsonSerializer.Deserialize<StockCountSessionEvent>(json);
            Console.WriteLine($" [x] {stockCountEvent?.SessionId}");
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
