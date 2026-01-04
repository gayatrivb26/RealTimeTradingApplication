

using MediatR;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Orders.Dtos;

namespace RealTimeTradingApp.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersQuery: IRequest<PagedResult<OrderDto>>
    {
        public Guid UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
