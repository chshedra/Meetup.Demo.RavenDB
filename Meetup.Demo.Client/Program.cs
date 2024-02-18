﻿using Meetup.Demo.Client;
using Meetup.Demo.Common.Postgres;
using Meetup.Demo.Common.RavenDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    private static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        host.Run();
        Console.ReadLine();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(
                (hostContext, services) =>
                {
                    services.AddSingleton<IDocumentStoreHolder, DocumentStoreHolder>();
                    services.AddTransient<AppDbContext>();
                    services.AddHostedService<StockCountEventProducer>();
                    services.AddHostedService<RequestProducer>();
                }
            );
}
