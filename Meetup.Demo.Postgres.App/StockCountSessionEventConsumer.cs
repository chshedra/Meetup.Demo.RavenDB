using System.Text;
using System.Text.Json;
using Meetup.Demo.Common.MessageBroker;
using Meetup.Demo.Domain;
using Microsoft.EntityFrameworkCore;
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
        consumer.Received += async (model, e) =>
        {
            byte[] body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var stockCountEvent = JsonSerializer.Deserialize<StockCountSessionEvent>(json);

            if (stockCountEvent != null)
            {
                var stockCount = new StockCount
                {
                    Id = stockCountEvent.StockCountId,
                    StartedAt = stockCountEvent.StartedAt,
                    Status = stockCountEvent.Status
                };

                try
                {
                    await _dbContext.AddAsync(stockCount);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine($" [x] Event consumed {stockCountEvent?.SessionId}");
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
