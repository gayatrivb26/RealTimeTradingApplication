using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Repositories
{
    public class OrderRepository: IOrderRepository
    {
        private readonly TradingDbContext _context;

        public OrderRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order, CancellationToken ct)
        {
            await _context.Orders.AddAsync(order, ct);
            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Orders
                .Include(o => o.Stock)
                .Include(o => o.Portfolio)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<Order> UpdateAsync(Order order, CancellationToken ct)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(ct);
            return order;
        }

        public async Task<List<Order>> GetOpenOrdersBySymbolAsync(string symbol, CancellationToken ct)
        {
            return await _context.Orders
                .Where(o => o.Symbol == symbol &&
                            o.RemainingQuantity > 0 &&
                            (o.Status == OrderStatus.New || o.Status == OrderStatus.PartiallyFilled))
                .OrderBy(o => o.CreatedAt) // FIFO for same price
                .ToListAsync(ct);
        }

        public async Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt) 
                .ToListAsync(ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            return _context.SaveChangesAsync(ct);
        }

        public async Task<List<Order>> GetOpenOrdersForMatchingAsync(string symbol, OrderSide side, CancellationToken ct)
        {
            return await _context.Orders
                .Where(o =>
                o.Symbol == symbol &&
                o.Side == side &&
                (o.Status == OrderStatus.New || o.Status == OrderStatus.PartiallyFilled) && o.RemainingQuantity > 0)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
