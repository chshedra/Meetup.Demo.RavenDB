﻿namespace Meetup.Demo.RavenDB.Domain;

public class StockCountSessionEvent
{
    public string SessionId { get; set; }

    public string StockCountId { get; set; }

    public string Status { get; set; }
}
