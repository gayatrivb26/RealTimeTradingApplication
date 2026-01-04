using MediatR;
using RealTimeTradingApp.Application.Features.Holdings.Dtos;


namespace RealTimeTradingApp.Application.Features.Holdings.Queries
{
    public class GetPortfolioHoldingsQuery: IRequest<List<HoldingDto>>
    {
        public Guid PortfolioId { get; set; }
    }
}
