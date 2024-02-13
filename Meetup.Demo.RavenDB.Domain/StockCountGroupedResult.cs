namespace Meetup.Demo.Domain;

public class StockCountGroupedResult
{
    public string ProductId { get; set; }

    public string Description { get; set; }

    public Dictionary<string, int> ZoneCounts { get; set; }

    public int Count { get; set; }
}
