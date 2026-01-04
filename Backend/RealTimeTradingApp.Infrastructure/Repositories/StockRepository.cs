using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Repositories
{
    public class StockRepository: IStockRepository
    {
        private readonly TradingDbContext _context;

        public StockRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken ct)
        {
            return await _context.Stocks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Symbol == symbol, ct);
        }

        public async Task<Stock?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Stocks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task<List<Stock>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Stocks
                .AsNoTracking()
                .OrderBy(s => s.Symbol)
                .ToListAsync(ct);
        }
    }
}
