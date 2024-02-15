using System.Text;
using System.Text.Json;
using Meetup.Demo.Common.MessageBroker;
using Meetup.Demo.Common.Postgres;
using Meetup.Demo.Domain;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.Postgres.App;

public class StockCountReadEventConsumer : EventConsumerBase
{
    private readonly AppDbContext _dbContext;

    public StockCountReadEventConsumer(AppDbContext dbContext)
        : base(nameof(StockCountReadEvent))
    {
        _dbContext = dbContext;
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
                var things = stockCountEvent.Things.Select(x => new Thing
                {
                    Id = x.ThingId,
                    ProductId = x.ProductId,
                    ZoneId = x.ZoneId,
                    StockCountId = stockCountEvent.StockCountId
                });

                foreach (var thing in things)
                {
                    var product = await _dbContext.Products.FirstAsync(x =>
                        x.Id == thing.ProductId
                    );
                }

                var stockCount = await _dbContext.StockCounts.FirstAsync(x =>
                    x.Id == stockCountEvent.StockCountId
                );

                if (stockCount.Things == null)
                {
                    stockCount.Things = new List<Thing>(things);
                }
                else
                {
                    stockCount.Things.AddRange(things);
                }

                Console.WriteLine($" [x] Batch consumed: {stockCountEvent?.BatchId}");
            }
            await _dbContext.SaveChangesAsync();
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
