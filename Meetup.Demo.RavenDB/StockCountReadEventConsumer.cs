using System.Text;
using System.Text.Json;
using Meetup.Demo.RavenDB.Domain;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.RavenDB.App;

public class StockCountReadEventConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IDocumentStoreHolder _documentStoreHolder;

    public StockCountReadEventConsumer(IDocumentStoreHolder documentStoreHolder)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: nameof(StockCountReadEvent), type: ExchangeType.Fanout);

        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(
            queue: _queueName,
            exchange: nameof(StockCountReadEvent),
            routingKey: string.Empty
        );
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
