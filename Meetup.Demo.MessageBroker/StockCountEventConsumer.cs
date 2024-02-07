using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Meetup.Demo.MessageBroker;

public class StockCountEventConsumer : IDisposable
{
    private bool _disposed = false;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public event EventHandler<BasicDeliverEventArgs>? StockCountEventReceived;

    public StockCountEventConsumer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void ConsumeStockCountMessage<T>()
    {
        _channel.ExchangeDeclare(exchange: typeof(T).Name, type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: queueName, exchange: typeof(T).Name, routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, e) =>
        {
            StockCountEventReceived?.Invoke(model, e);
        };
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            _connection.Close();
            _channel.Close();
        }
        _disposed = true;
    }

    ~StockCountEventConsumer()
    {
        Dispose(false);
    }
}
