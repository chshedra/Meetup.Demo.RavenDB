using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meetup.Demo.RavenDB.Domain;

public class StockCountReadEvent
{
    public string Id { get; set; }

    public string StockCountId { get; set; }

    public string SessionId { get; set; }

    public string BatchId { get; set; }

    public List<ThingReadInfo> ThingsBatch { get; set; }
}
