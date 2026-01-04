
using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Features.Orders.Dtos;
using RealTimeTradingApp.Application.Features.Orders.Queries.GetOpenOrders;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetOpenOrdersQueryHandler: IRequestHandler<GetOpenOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOpenOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(GetOpenOrdersQuery request, CancellationToken ct)
        {
            var openOrders = await _orderRepository.GetOpenOrdersBySymbolAsync(request.Symbol, ct);

            return _mapper.Map<List<OrderDto>>(openOrders);
        }
    }
}
