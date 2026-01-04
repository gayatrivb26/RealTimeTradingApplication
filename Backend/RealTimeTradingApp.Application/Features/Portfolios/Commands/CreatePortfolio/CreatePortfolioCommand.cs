using MediatR;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;

namespace RealTimeTradingApp.Application.Features.Portfolios.Commands.CreatePortfolio
{
    public class CreatePortfolioCommand : IRequest<PortfolioDto>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = default!;

    }
}
