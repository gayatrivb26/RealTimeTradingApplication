using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Repositories
{
    public class TradeRepository: ITradeRepository
    {
        private readonly TradingDbContext _context;

        public TradeRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task<Trade> AddAsync(Trade trade, CancellationToken ct)
        {
            await _context.Trades.AddAsync(trade, ct);
            return trade;
        }

        public async Task<List<Trade>> GetTradesByUserAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Trades
                .Where(t =>
                    _context.Orders.Any(o => o.Id == t.BuyOdrderId && o.UserId == userId) ||
                    _context.Orders.Any(o => o.Id == t.SellOdrderId && o.UserId == userId)
                )
                .OrderByDescending(t => t.ExecutedAt)
                .ToListAsync(ct);
        }

        public async Task<List<Trade>> GetTradesByOrderAsync(Guid orderId, CancellationToken ct)
        {
            return await _context.Trades
                .Where(t => t.BuyOdrderId == orderId ||
                            t.SellOdrderId== orderId)
                .OrderByDescending(t => t.ExecutedAt)
                .ToListAsync(ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            return _context.SaveChangesAsync(ct);
        }

        public IQueryable<Trade> Query()
        {
            return _context.Trades.AsNoTracking();
        }

        public async Task AddRangeAsync(IEnumerable<Trade> trades, CancellationToken ct)
        {
            await _context.Trades.AddRangeAsync(trades, ct);
        }
    }
}
