﻿using Meetup.Demo.Common.Postgres;
using Meetup.Demo.Postgres.App;
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
                    services.AddTransient<DbContext, AppDbContext>();
                    services.AddHostedService<StockCountReadEventConsumer>();
                    services.AddHostedService<StockCountSessionEventConsumer>();
                }
            );
}
