using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken cancellationToken);
        Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Portfolio>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken);
    }
}
