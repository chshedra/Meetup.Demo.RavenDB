using System.Text.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Meetup.Demo.Assets;

public class Asset
{
    public string Id { get; set; }

    public string Zone { get; set; }

    public string Size { get; set; }

    public DateTime LastUpdate { get; set; }

    public string ProductId { get; set; }

    [BsonExtraElements]
    public JsonDocument ExtraElements { get; set; }
}
