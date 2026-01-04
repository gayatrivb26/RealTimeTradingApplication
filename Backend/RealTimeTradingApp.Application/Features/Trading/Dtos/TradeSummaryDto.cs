
namespace RealTimeTradingApp.Application.Features.Trading.Dtos
{
    public class TradeSummaryDto
    {
        public int TotalTrades { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalBuyVolume { get; set; }
        public decimal TotalSellVolume { get; set;} 
        public decimal? RealizedPnL {  get; set; }
    }
}
