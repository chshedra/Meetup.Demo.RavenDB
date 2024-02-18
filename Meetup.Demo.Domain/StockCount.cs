using System.Text.Json.Serialization;

namespace Meetup.Demo.Domain;

public class StockCount
{
    public string Id { get; set; }

    public DateTime StartedAt { get; set; }

    public string Status { get; set; }

    [JsonIgnore]
    public List<Thing> Things { get; set; }
}
