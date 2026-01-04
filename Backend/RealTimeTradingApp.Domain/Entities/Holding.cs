
namespace RealTimeTradingApp.Domain.Entities
{
    public class Holding
    {
        public Guid Id { get; set; }
        public Guid PortfolioId { get; set; }
        public Guid StockId { get; set; }
        public decimal Quantity { get; set; }
        public decimal AverageCost { get; set; }
        public string Sector { get; set; } = default!;
        //public decimal RealizedPnL { get; set; }

        // Navigation
        public Portfolio Portfolio { get; set; } = default!;
        public Stock Stock { get; set; } = default!;

        // Computed proprties
        public decimal CurrentValue => Quantity * Stock.CurrentPrice;
        public decimal UnrealizedPnL
        {
            get
            {
                if (Quantity > 0)
                    return (Stock.CurrentPrice - AverageCost) * Quantity;
                else
                    return (AverageCost - Stock.CurrentPrice) * Math.Abs(Quantity);
            }
        }

        // Domain behavior
        public void BuyShares(decimal qty, decimal price)
        {
            if (qty < 0) throw new ArgumentException("Quantity must be positive.");

            if(Quantity < 0)
            {
                Quantity += qty;
                return;
            }

            var totalCostBefore = Quantity * AverageCost;
            var totalCostAfter = totalCostBefore + (qty * price);

            Quantity += qty;
            AverageCost = totalCostAfter / Quantity;
        }

        public void ShortSell(decimal qty, decimal price)
        {
            if (qty <= 0) throw new ArgumentException("Quantity must be positive.");

            Quantity -= qty;

            if (AverageCost == 0)
                AverageCost = price;
        }

        public void SellShares(decimal qty)
        {
            if (qty <= 0) throw new ArgumentException("Quantity must be positive.");

            Quantity -= qty;
        }
    }
}
