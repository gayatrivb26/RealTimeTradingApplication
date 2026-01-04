
using MediatR;
using RealTimeTradingApp.Application.Features.Trading.Dtos;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradesBySymbol
{
    public class GetTradesBySymbolQuery: IRequest<List<TradeDto>>
    {
        public string Symbol { get; set; } = default!;
        public Guid? PortfolioId { get; set; }
    }
}
