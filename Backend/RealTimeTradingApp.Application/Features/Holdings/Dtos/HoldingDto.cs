
namespace RealTimeTradingApp.Application.Features.Holdings.Dtos
{
    public class HoldingDto
    {
        public string Symbol { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal AvgCost { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal UnrealizedPnL {  get; set; }
        public decimal RealizedPnL { get; set; }
        public decimal PnLPercentage { get; set; }
        public string Sector { get; set; } = default!;
    }
}
