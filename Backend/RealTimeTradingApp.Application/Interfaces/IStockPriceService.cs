namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IStockPriceService
    {
        Task<decimal?> GetLivePriceAsync(string symbol, CancellationToken ct);
    }
}
