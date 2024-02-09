using System.Text;
using System.Text.Json;
using Meetup.Demo.RavenDB.Domain;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.RavenDB.App;

public class StockCountSessionEventConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public StockCountSessionEventConsumer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: nameof(StockCountSessionEvent),
            type: ExchangeType.Fanout
        );

        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(
            queue: _queueName,
            exchange: nameof(StockCountSessionEvent),
            routingKey: string.Empty
        );
    }

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
