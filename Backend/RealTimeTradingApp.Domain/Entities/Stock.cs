

namespace RealTimeTradingApp.Domain.Entities
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public string Sector { get; set; } = default!;
        public decimal CurrentPrice { get; set; } = default!;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
    }
}
