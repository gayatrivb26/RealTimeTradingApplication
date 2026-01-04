

using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Orders.Commands.CancelOrder;
using RealTimeTradingApp.Application.Features.Orders.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Features.Orders.Commands.Cancel_Order
{
    public class CancelOrderCommandHandler: IRequestHandler<CancelOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CancelOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository  = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);
            if(order == null)
            {
                throw new NotFoundException($"Order with ID {request.OrderId} not found.");
            }

            if (order.UserId != request.UserId)
                throw new CustomValidationException("You can only cancel your own orders.");

            if (order.Status != OrderStatus.Pending)
                throw new CustomValidationException("Only pending orders can be cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;

            var updated = await _orderRepository.UpdateAsync(order, ct);

            return _mapper.Map<OrderDto>(updated);
        }
    }
}
