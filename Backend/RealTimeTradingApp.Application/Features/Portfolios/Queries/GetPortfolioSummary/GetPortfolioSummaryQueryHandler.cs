
using MediatR;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Portfolios.Queries.GetPortfolioSummary
{
    public class GetPortfolioSummaryQueryHandler: IRequestHandler<GetPortfolioSummaryQuery, PortfolioSummaryDto>
    {
        private readonly IHoldingRepository _holdingRepository;

        public GetPortfolioSummaryQueryHandler(IHoldingRepository holdingRepository)
        {
            _holdingRepository = holdingRepository;
        }

        public async Task<PortfolioSummaryDto> Handle(GetPortfolioSummaryQuery request, CancellationToken ct)
        {
            var holdings = await _holdingRepository.GetHoldingsForPortfolioAsync(request.PortfolioId, ct);

            if (holdings.Count == 0)
                throw new NotFoundException($"Portfolio with ID '{request.PortfolioId}' not found.");

            var totalInvested = holdings.Sum(h => h.AverageCost * Math.Abs(h.Quantity));
            var totalCurrent = holdings.Sum(h => h.Stock.CurrentPrice * h.Quantity);
            var pnl = totalCurrent - totalInvested;
            var returnPct = totalInvested > 0 ? (pnl / totalInvested) * 100 : 0;

            return new PortfolioSummaryDto
            {
                PortfolioId = request.PortfolioId,
                TotalInvestedValue = totalInvested,
                TotalCurrentValue = totalCurrent,
                TotalUnrealizedPnL = pnl,
                ReturnPercentage = returnPct
            };
        }
    }
}
