﻿using System.Text;
using System.Text.Json;
using Meetup.Demo.MessageBroker;
using Meetup.Demo.RavenDB.Domain;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.Postgres.App;

public class StockCountReadEventConsumer : EventConsumerBase
{
    private readonly DbContext _dbContext;

    public StockCountReadEventConsumer(DbContext dbContext)
        : base(nameof(StockCountReadEvent))
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
            var stockCountEvent = JsonSerializer.Deserialize<StockCountReadEvent>(json);

            using var dbContext = new AppDbContext();

            if (stockCountEvent != null)
            {
                dbContext.Add(stockCountEvent);
                Console.WriteLine($" [x] Batch {stockCountEvent?.BatchId}");
            }
            dbContext.SaveChanges();
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
