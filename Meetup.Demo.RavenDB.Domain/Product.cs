using System.Text.Json.Serialization;

namespace Meetup.Demo.Domain;

public class Product
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    [JsonIgnore]
    public List<ThingReadInfo> Things { get; set; }
}
