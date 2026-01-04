using MediatR;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;


namespace RealTimeTradingApp.Application.Features.Portfolios.Queries.GetPortfolioById
{
    public class GetPortfolioByIdQuery: IRequest<PortfolioDto>
    {
        public Guid Id { get; set; }
    }
}
