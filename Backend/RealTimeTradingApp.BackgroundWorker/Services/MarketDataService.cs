using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using RealTimeTradingApp.API.Hubs;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Infrastructure.Data;
using System.Net.Http.Json;

namespace RealTimeTradingApp.BackgroundWorker.Services
{
    public class MarketDataService: BackgroundService
    {
        private readonly ILogger<MarketDataService> _logger;
        private readonly IConfiguration _config;
        //private readonly IHubContext<StockPriceHub> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncRetryPolicy _retryPolicy;

        public MarketDataService(
            ILogger<MarketDataService> logger,
            IConfiguration config,
            //IHubContext<StockPriceHub> hubContext,
            IHttpClientFactory httpClientFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _config = config;
            //_hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
            _serviceScopeFactory = serviceScopeFactory;

            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                }, (ex, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(ex, $"Retry {retryCount} after {timespan} due to {ex.Message}");
                });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var apiKey = _config["Finnhub:apiKey"]; // Store in user secrets

            if (string.IsNullOrWhiteSpace(apiKey))
                _logger.LogWarning("Finnhub:apikey is not configured. MarketDataService will run but API call will fail.");

            // run until cancellation
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
                        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();


                        // Read all symbols once per loop
                        var stocks = await db.Stocks.AsNoTracking().ToListAsync(stoppingToken);
                        if (!stocks.Any())
                        {
                            _logger.LogInformation("No stocks found in DB to poll.");
                            return;
                        }

                        var client = _httpClientFactory.CreateClient("finnhub"); // configure base address is desired

                        // Bounded concurrencyto respect rate limits
                        var semaphore = new SemaphoreSlim(5);

                        var tasks = stocks.Select(s =>
                            FetchPriceForSymbolAsync(s.Symbol, s.Id, client, apiKey, semaphore, stoppingToken)).ToList();
                        //var tasks = new List<Task<(string symbol, decimal price, Guid stockId)?>>();

                        var results = await Task.WhenAll(tasks);

                        //foreach (var s in stocks)
                        //{
                        //    tasks.Add(FetchPriceForSymbolAsync(s.Symbol, s.Id, client, apiKey, semaphore, stoppingToken));
                        //}

                        //var results = await Task.WhenAll(tasks);

                        // Prepare DB updates in a single DbContext scope (to save changes in one SaveChanges)
                        //using var updateScope = _services.CreateScope();
                        //var updateDb = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

                        //var broadcasts = new List<(string symbol, decimal price, Guid stockId)>();

                        foreach (var r in results.Where(x => x.HasValue).Select(x => x!.Value))
                        {
                            // fetch tracked entity for update
                            var stock = await db.Stocks.FirstOrDefaultAsync(s => s.Id == r.stockId, stoppingToken);
                            if (stock == null) continue;

                            stock.CurrentPrice = r.price;
                            stock.LastUpdated = DateTime.UtcNow;
                            //broadcasts.Add((r.symbol, r.price, r.stockId));

                            // updateDb cache
                            await cache.SetAsync($"stock:price:{r.symbol}", r.price, TimeSpan.FromSeconds(30), stoppingToken);
                            //await _cacheService.SetAsync($"stock:price:{r.symbol}", r.price, TimeSpan.FromSeconds(30), stoppingToken);

                            //await _hubContext.Clients.Group($"stock:{r.symbol}")
                            //    .SendAsync("PriceUpdated", new { r.symbol, r.price }, stoppingToken);
                        }

                        await db.SaveChangesAsync(stoppingToken);

                        //if (broadcasts.Any())
                        //{
                        //    await updateDb.SaveChangesAsync(stoppingToken);

                        //    // Broadcast grouped updates to reduce SignalR messages
                        //    foreach (var b in broadcasts)
                        //    {
                        //        // Broadcasr to stock group
                        //        var updatePayload = new { Symbol = b.symbol, Price = b.price, TimeStamp = DateTime.UtcNow };
                        //        await _hubContext.Clients.Group($"stock:{b.symbol}")
                        //        .SendAsync("PriceUpdated", updatePayload, cancellationToken: stoppingToken);
                        //    }

                        //}

                        //foreach (var symbol in symbols)
                        //{
                        //    var url = $"https:finnhub.io/api/v1/quote?symbol={symbol}&token={apiKey}";
                        //    var response = await httpClient.GetFromJsonAsync<FinnhubQuote>(url, stoppingToken);
                        //    var price = response?.C ?? 0;

                        //    if (price > 0)
                        //    {
                        //        // Update DB
                        //        var stock = await db.Stocks.FirstAsync(s => s.Symbol == symbol, stoppingToken);
                        //        stock.CurrentPrice = price;
                        //        stock.LastUpdated = DateTime.UtcNow;
                        //        await db.SaveChangesAsync(stoppingToken);

                        //        // Cache in Redis
                        //        var cacheKey = $"stock:price:{symbol}";
                        //        await _cache.SetStringAsync(cacheKey, price.ToString(), new DistributedCacheEntryOptions
                        //        {
                        //            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                        //        }, stoppingToken);

                        //        // Broadcast via SignalR
                        //        var update = new { Symbol = symbol, Price = price, Timestamp = DateTime.UtcNow };
                        //        await _hubContext.Clients.Group($"stock:{symbol}").SendAsync("PriceUpdated", update, stoppingToken);

                        //        // Also push to portfolios holding this stock (for P&L updates)
                        //        var portfoliosWithStock = await db.Holdings
                        //            .Where(h => h.StockId == stock.Id)
                        //            .Select(h => h.PortfolioId)
                        //            .Distinct()
                        //            .ToListAsync(stoppingToken);

                        //        foreach (var portfolioId in portfoliosWithStock)
                        //        {
                        //            await _hubContext.Clients.Group($"portfolio:{portfolioId}").SendAsync("PortfolioUpdated",
                        //                new { PortfolioId = portfolioId, Symbol = symbol, NewPrice = price }, stoppingToken);
                        //        }
                        //    }
                        //}
                    });
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "MarketDataService caught exception after retries.");
                }

                // Wait before next poll
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (TaskCanceledException) { /*Shutdown requested*/}
                    
                    //await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Poll every 10s, like Groww feeds
            
            }
        }

        private async Task<(string symbol, decimal price, Guid stockId)?> FetchPriceForSymbolAsync(
            string symbol,
            Guid stockId,
            HttpClient client,
            string apiKey,
            SemaphoreSlim semaphore,
            CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                    return null;

                var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={apiKey}";
                try
                {
                    var resp = await client.GetFromJsonAsync<FinnhubQuote>(url, cancellationToken);
                    var price = resp?.C ?? 0m;
                    if (price > 0) return (symbol, price, stockId);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to fetch price for {symbol}");
                }
            }
            finally
            {
                semaphore.Release();
            }

            return null;
        }
    }

    public record FinnhubQuote(decimal C); //Current price
}
