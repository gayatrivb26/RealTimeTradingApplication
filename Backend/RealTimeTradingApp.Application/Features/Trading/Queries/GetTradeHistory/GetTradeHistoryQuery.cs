

using MediatR;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Trading.Dtos;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeHistory
{
    public class GetTradeHistoryQuery: IRequest<PagedResult<TradeDto>>
    {
        public Guid? PortfolioId { get; set; }
        public Guid? UserId { get; set; }
        public string? Symbol { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
