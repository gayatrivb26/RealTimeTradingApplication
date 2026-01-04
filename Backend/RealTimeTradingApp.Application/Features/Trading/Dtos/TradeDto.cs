
using System.Globalization;

namespace RealTimeTradingApp.Application.Features.Trading.Dtos
{
    public class TradeDto
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime ExecutedAt { get; set; }

        public Guid BuyOrderId { get; set; }
        public Guid SellOrderId { get; set; }

        public Guid PortfolioId { get; set; }
    }
}
