using RealTimeTradingApp.Application.Features.Holdings.Dtos;

namespace RealTimeTradingApp.Application.Features.Portfolios.Dtos
{
    public class PortfolioDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string UserFullName { get; set; } = default!;
        public decimal UserBalance { get; set; }
        public decimal TotalPortfolioValue { get; set; }
        public decimal UnrealizedPnL {  get; set; }
        public decimal XIRR { get; set; }

        public Dictionary<string, decimal> Allocation { get; set; } = new();
        public List<HoldingDto> Holdings { get; set; } = new();
    }
}
