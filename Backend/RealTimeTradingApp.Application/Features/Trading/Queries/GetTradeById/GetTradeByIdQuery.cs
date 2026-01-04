

using MediatR;
using RealTimeTradingApp.Application.Features.Trading.Dtos;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeById
{
    public class GetTradeByIdQuery: IRequest<TradeDto>
    {
        public Guid TradeId { get; set; }
    }
}
