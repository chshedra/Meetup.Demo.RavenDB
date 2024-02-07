namespace Meetup.Demo.MessageBroker;

public class StockCountSessionEvent
{
    public string SessionId { get; set; }

    public string StockCountId { get; set; }

    public string Status { get; set; }
}
