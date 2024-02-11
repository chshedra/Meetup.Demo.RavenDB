using System.Text.Json.Serialization;

namespace Meetup.Demo.Domain;

public class ThingReadInfo
{
    public string Id { get; set; }

    public string ThingId { get; set; }

    public string ProductId { get; set; }

    public string UserId { get; set; }

    public string ZoneId { get; set; }

    [JsonIgnore]
    public string StockCountReadEventId { get; set; }

    [JsonIgnore]
    public StockCountReadEvent StockCountReadEvent { get; set; }
}
