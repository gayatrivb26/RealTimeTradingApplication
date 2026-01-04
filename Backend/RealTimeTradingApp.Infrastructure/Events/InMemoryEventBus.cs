

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealTimeTradingApp.Application.Common.Events;

namespace RealTimeTradingApp.Infrastructure.Events
{
    public class InMemoryEventBus: IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InMemoryEventBus> _logger;

        public InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
            where TEvent: IIntegrationEvent
        {
            if(@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventType = @event.GetType().Name;
            _logger.LogInformation($"Publishing integration event {eventType} with Id {@event.Id}");

            using var scope = _serviceProvider.CreateScope();

            var handlers = scope.ServiceProvider
                .GetServices<IIntegrationEventHandler<TEvent>>()
                .ToList();

            if (!handlers.Any())
            {
                _logger.LogWarning($"No handlers registered for event type {eventType}");
                return;
            }

            foreach(var handler in handlers )
            {
                try
                {
                    await handler.HandleAsync(@event, ct);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Error while handling event {eventType} by {handler.GetType().Name}");
                }
            }
        }
    }
}
