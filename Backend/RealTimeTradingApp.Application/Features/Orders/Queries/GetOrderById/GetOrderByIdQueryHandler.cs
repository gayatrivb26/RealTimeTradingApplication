
using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Orders.Dtos;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler: IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);

            if (order == null)
                throw new NotFoundException($"Order with ID '{request.OrderId}' not found.");

            return _mapper.Map<OrderDto>(order);
        }
    }
}
