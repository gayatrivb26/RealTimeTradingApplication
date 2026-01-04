
using MediatR;
using RealTimeTradingApp.Application.Features.Orders.Dtos;

namespace RealTimeTradingApp.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery: IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
    }
}
