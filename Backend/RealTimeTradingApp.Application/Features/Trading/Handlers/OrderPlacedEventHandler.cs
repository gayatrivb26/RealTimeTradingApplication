
using Microsoft.Extensions.Logging;
using RealTimeTradingApp.Application.Common.Events;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Trading.Handlers
{
    public class OrderPlacedEventHandler: 
        IIntegrationEventHandler<OrderPlacedEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMachingEngineService _machingEngine;
        private readonly ILogger<OrderPlacedEventHandler> _logger;

        public OrderPlacedEventHandler(
            IOrderRepository orderRepository,
            IMachingEngineService machingEngine,
            ILogger<OrderPlacedEventHandler> logger)
        {
            _orderRepository = orderRepository;
            _machingEngine = machingEngine;
            _logger = logger;
        }

        public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct)
        {
            _logger.LogInformation($"Handling OrderPlacedEvent for OrderId {@event.OrderId}, Symbol {@event.Symbol}");

            // Get thefull order from repository
            var order = await _orderRepository.GetByIdAsync(@event.OrderId, ct);
            if(order == null)
            {
                _logger.LogInformation($"Order with Id {@event.OrderId} not found when handling OrderPlacedEvent");
                return;
            }

            // Only match orders that are still open/new
            if(order.RemainingQuantity <= 0 || order.Status != Domain.Enums.OrderStatus.New)
            {
                _logger.LogInformation($"Order {@event.OrderId} is not eligible for matching.");
                return;
            }

            // Call the matching engine
            var trades = await _machingEngine.MatchAsync(order, ct);

            _logger.LogInformation($"Matching engine produced {trades.Count} trades for OrderId {order.Id}");
        }
    }
}
