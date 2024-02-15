using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Meetup.Demo.Common.MessageBroker;

public abstract class EventConsumerBase : BackgroundService
{
    private readonly IConnection _connection;
    protected readonly IModel _channel;
    protected readonly string _queueName;

    public EventConsumerBase(string exchangeName)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            DispatchConsumersAsync = true
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName, exchange: exchangeName, routingKey: string.Empty);
    }
}
