using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Repositories
{
    public class HoldingRepository: IHoldingRepository
    {
        private readonly TradingDbContext _context;

        public HoldingRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task<Holding?> GetByPortfolioAndStockAsync(Guid portfolioId, Guid stockId, CancellationToken ct)
        {
            return await _context.Holdings
                .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.StockId == stockId, ct);
        }

        public async Task AddAsync(Holding holding, CancellationToken ct)
        {
            await _context.Holdings.AddAsync(holding, ct);
        }

        public Task UpdateAsync(Holding holding, CancellationToken ct)
        {
            _context.Holdings.Update(holding);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Holding holding, CancellationToken ct)
        {
            _context.Holdings.Remove(holding);
            return Task.CompletedTask;
        }

        public async Task<List<Holding>> GetHoldingsForPortfolioAsync(Guid portfolioId, CancellationToken ct)
        {
            return await _context.Holdings
                .Include(h => h.Stock)
                .Where(h => h.PortfolioId == portfolioId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
