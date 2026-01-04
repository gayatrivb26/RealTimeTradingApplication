using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Orders.Dtos;
using RealTimeTradingApp.Application.Features.Users.Queries.GetAllUsers;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetUserOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<OrderDto>> Handle(GetUserOrdersQuery request, CancellationToken ct)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(request.UserId, ct);

            var totalCount = orders.Count;

            var pagedData = orders
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var items = _mapper.Map<List<OrderDto>>(pagedData);

            return new PagedResult<OrderDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
