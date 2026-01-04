using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface ITradeRepository
    {
        Task<Trade> AddAsync(Trade trade, CancellationToken ct);
        Task<List<Trade>> GetTradesByUserAsync(Guid userId, CancellationToken ct);
        Task<List<Trade>> GetTradesByOrderAsync(Guid orderd, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        IQueryable<Trade> Query();

        Task AddRangeAsync(IEnumerable<Trade> trades, CancellationToken ct);

    }
}
