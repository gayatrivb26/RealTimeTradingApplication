using RealTimeTradingApp.BackgroundWorker.Services;
using RealTimeTradingApp.Infrastructure;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/bg-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting background worker host...");

    IHost host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            var config = context.Configuration;

            services.AddInfrastructureServices(config);

            // Register IHttpClientFactory and a named client for finnhub
            services.AddHttpClient("finnub", client =>
            {
                client.BaseAddress = new Uri("https://finnhub.io");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddHostedService<MarketDataService>();
        })
        .Build();
    
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Background worker terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}