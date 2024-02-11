using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Meetup.Demo.Common.MessageBroker;

public class EventProducer
{
    public void SendStockCountMessage<T>(T message)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: typeof(T).Name, type: ExchangeType.Fanout);

        var stockCountEventJson = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(stockCountEventJson);
        channel.BasicPublish(
            exchange: typeof(T).Name,
            routingKey: string.Empty,
            basicProperties: null,
            body: body
        );
    }
}
