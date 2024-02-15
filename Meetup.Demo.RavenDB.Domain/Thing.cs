using Newtonsoft.Json;

namespace Meetup.Demo.Domain;

public class Thing
{
    public string Id { get; set; }

    public string ZoneId { get; set; }

    public string ProductId { get; set; }

    [JsonIgnore]
    public Product Product { get; set; }

    public string StockCountId { get; set; }

    [JsonIgnore]
    public StockCount StockCount { get; set; }
}
