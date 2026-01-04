using Microsoft.Extensions.Caching.Distributed;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Infrastructure.Services
{
    public class StockPriceService: IStockPriceService
    {
        private readonly IDistributedCache _cache;

        public StockPriceService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<decimal?> GetLivePriceAsync(string symbol, CancellationToken ct)
        {
            var cacheKey = $"stock:price:{symbol}";
            var value = await _cache.GetStringAsync(cacheKey, ct);

            if(decimal.TryParse(value, out var price))
            {
                return price;
            }
            return null;
        }
    }
}
