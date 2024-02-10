using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Meetup.Demo.RavenDB.App;

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
                    services.AddHostedService<StockCountReadEventConsumer>();
                    services.AddHostedService<StockCountSessionEventConsumer>();
                }
            );
}
