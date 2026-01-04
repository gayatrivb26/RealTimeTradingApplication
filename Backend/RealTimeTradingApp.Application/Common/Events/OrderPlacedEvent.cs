using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Common.Events
{
    public class OrderPlacedEvent: IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccuredAt { get; } = DateTime.UtcNow;

        public Guid OrderId { get; init; }
        public Guid UserId { get; init; }
        public Guid PortfolioId { get; init; }
        public string Symbol { get; init; } = default!;
        public decimal Quantity { get; init; }
        public decimal? Price { get; init; }
        public OrderSide Side { get; init; }
        public OrderType Type { get; init; }
    }
}
