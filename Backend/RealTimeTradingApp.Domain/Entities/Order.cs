
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PortfolioId { get; set; }
        public Guid StockId { get; set; }

        public string Symbol { get; set; } = default!;

        public OrderSide Side { get; set; }
        public OrderType Type { get; set; }
        public TimeInForce TimeInForce { get; set; } = TimeInForce.Day;
        public OrderStatus Status { get; set; } = OrderStatus.New;

        public decimal Quantity { get; set; }
        public decimal? Price { get; set; } // Used only for limit orders
        public decimal RemainingQuantity { get; set; }
       
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Portfolio Portfolio { get; set; } = default!;
        public Stock Stock { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

    }
}
