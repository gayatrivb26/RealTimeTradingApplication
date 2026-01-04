
using MediatR;
using RealTimeTradingApp.Application.Features.Trading.Dtos;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeSummary
{
    public class GetTradeSummaryQuery: IRequest<TradeSummaryDto>
    {
        public Guid? PortfolioId { get; set; }
        public Guid? UserId { get; set; }
    }
}
