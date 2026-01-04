

using MediatR;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;

namespace RealTimeTradingApp.Application.Features.Portfolios.Queries.GetPortfolioSummary
{
    public class GetPortfolioSummaryQuery: IRequest<PortfolioSummaryDto>
    {
        public Guid PortfolioId { get; set; }
    }
}
