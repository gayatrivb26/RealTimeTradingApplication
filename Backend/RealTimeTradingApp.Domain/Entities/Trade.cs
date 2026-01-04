
namespace RealTimeTradingApp.Domain.Entities
{
    public class Trade
    {
        public Guid Id { get; set; }
        public Guid BuyOdrderId { get; set; }
        public Guid SellOdrderId { get; set; }

        public Guid StockId { get; set; }
        public Guid PortfolioId { get; set; }
        public string Symbol { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }

        public Portfolio Portfolio { get; set; } = default!;
        public Stock Stock { get; set; } = default!;
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    }
}
