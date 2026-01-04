using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Common.Events;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Orders.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler: IRequestHandler<PlaceOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public PlaceOrderCommandHandler(
            IOrderRepository orderRepository,
            IPortfolioRepository portfolioRepository,
            IStockRepository stockRepository,
            IMapper mapper,
            IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _portfolioRepository = portfolioRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
            _eventBus = eventBus;
        }

        public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken ct)
        {
            // Check that portfolio exists and belongs to user
            var portfolio = await _portfolioRepository.GetByIdAsync(request.PortfolioId, ct);
            if (portfolio == null || portfolio.UserId !=  request.UserId)
            {
                throw new CustomValidationException("Invalid portfoli or user does not own this portfolio.");
            }

            // check that symbol exists
            var stock = await _stockRepository.GetBySymbolAsync(request.symbol, ct);
            if (stock == null)
                throw new NotFoundException($"Stock with symbol '{request.symbol}' not found.");

            // Map string side/type to enums
            if (!Enum.TryParse<OrderSide>(request.Side, ignoreCase: true, out var side))
                throw new CustomValidationException("Invalid order side.");

            if (!Enum.TryParse<OrderType>(request.OrderType, ignoreCase: true, out var orderType))
                throw new CustomValidationException("Invalid order type.");

            // Create domain entity
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PortfolioId = request.PortfolioId,
                StockId = stock.Id,
                Symbol = request.symbol,

                Side = side,
                Type = orderType,

                Quantity = request.Quantity,
                RemainingQuantity = request.Quantity,
                Price = request.LimitPrice ?? 0, //orderType == OrderType.Limit ? request.LimitPrice : null,

                Status = OrderStatus.New,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order, ct);
            await _orderRepository.SaveChangesAsync(ct);

            var ordrPlacedEvent = new OrderPlacedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                PortfolioId = order.PortfolioId,
                Symbol = order.Symbol,
                Quantity = order.Quantity,
                Price = order.Price,
                Side = order.Side,
                Type = order.Type
            };

            await _eventBus.PublishAsync(ordrPlacedEvent, ct);

            // 3. Return order with updated status / may be also trades in dto
            return _mapper.Map<OrderDto>(order);
        }

    }
}
