namespace Meetup.Demo.Domain;

public class StockCountSessionEvent
{
    public string Id { get; set; }

    public string SessionId { get; set; }

    public string StockCountId { get; set; }

    public string Status { get; set; }
}
