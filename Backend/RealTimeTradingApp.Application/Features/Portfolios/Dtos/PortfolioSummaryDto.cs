using RealTimeTradingApp.Application.Features.Holdings.Dtos;

namespace RealTimeTradingApp.Application.Features.Portfolios.Dtos
{
    public class PortfolioSummaryDto
    {
        public Guid PortfolioId { get; set; }
        public decimal TotalInvestedValue { get; set; }
        public decimal TotalCurrentValue { get; set; }
        public decimal TotalUnrealizedPnL { get; set; }
        public decimal ReturnPercentage { get; set; }
    }
}
