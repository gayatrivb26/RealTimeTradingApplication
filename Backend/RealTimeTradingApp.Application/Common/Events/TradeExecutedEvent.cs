using MediatR;
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Common.Events
{
    public class TradeExecutedEvent: IIntegrationEvent, INotification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime OccuredAt { get; } = DateTime.UtcNow;

        public Guid PortfolioId { get; init; }
        public Guid StockId { get; init; }

        public string Symbol { get; init; } = default!;

        public decimal Quantity { get; init; }
        public decimal Price { get; init; }

        public OrderSide Side { get; init; }
        public OrderType OrderType { get; init; }

        public Guid BuyOrderId { get; init; }
        public Guid SellOrderId { get; init; }

        public bool IsShortSell => Side == OrderSide.Sell;
    }
}
