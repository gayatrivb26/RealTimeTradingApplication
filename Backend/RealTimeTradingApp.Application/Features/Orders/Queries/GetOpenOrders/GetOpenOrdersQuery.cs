
using MediatR;
using RealTimeTradingApp.Application.Features.Orders.Dtos;

namespace RealTimeTradingApp.Application.Features.Orders.Queries.GetOpenOrders
{
    public class GetOpenOrdersQuery : IRequest<List<OrderDto>>
    {
        public string Symbol { get; set; } = default!;
    }
}
