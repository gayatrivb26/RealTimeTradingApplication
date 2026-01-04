using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order, CancellationToken ct);
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Order> UpdateAsync(Order order, CancellationToken ct);
        
        Task<List<Order>> GetOpenOrdersBySymbolAsync(string symbol, CancellationToken ct);
        Task<List<Order>> GetUserOrdersAsync(Guid userId, CancellationToken ct);
        Task<List<Order>> GetOpenOrdersForMatchingAsync(string symbol, OrderSide side, CancellationToken ct);

        Task SaveChangesAsync(CancellationToken ct);
    }
}
