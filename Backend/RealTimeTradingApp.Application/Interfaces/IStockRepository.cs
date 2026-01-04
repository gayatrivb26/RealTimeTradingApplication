using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IStockRepository
    {
        Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken ct);
        Task<Stock?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<Stock>> GetAllAsync(CancellationToken ct);
    }
}
