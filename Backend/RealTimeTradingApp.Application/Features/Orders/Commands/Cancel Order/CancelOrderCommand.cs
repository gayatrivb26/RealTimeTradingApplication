using MediatR;
using RealTimeTradingApp.Application.Features.Orders.Dtos;

namespace RealTimeTradingApp.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand: IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

    }
}
