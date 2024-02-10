using System.Text;
using System.Text.Json;
using Meetup.Demo.MessageBroker;
using Meetup.Demo.RavenDB.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.Postgres.App;

public class StockCountSessionEventConsumer : EventConsumerBase
{
    private readonly DbContext _dbContext;

    public StockCountSessionEventConsumer(DbContext dbContext)
        : base(nameof(StockCountSessionEvent))
    {
        _dbContext = dbContext;
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
