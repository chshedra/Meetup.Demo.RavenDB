namespace Meetup.Demo.Domain;

public class StockCountGroupedResult
{
    public string ProductId { get; set; }

    public string ProductName { get; set; }

    public string? ProductDescription { get; set; }

    public Dictionary<string, int> ZoneCounts { get; set; }

    public int Counted { get; set; }
}
