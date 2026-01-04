using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IHoldingRepository
    {
        Task<Holding> GetByPortfolioAndStockAsync(Guid portfolioId, Guid stockId, CancellationToken ct);
        Task AddAsync(Holding holding, CancellationToken ct);
        Task UpdateAsync(Holding holding, CancellationToken ct);
        Task DeleteAsync(Holding holding, CancellationToken ct);
        Task<List<Holding>> GetHoldingsForPortfolioAsync(Guid portfolioId, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);

    }
}
