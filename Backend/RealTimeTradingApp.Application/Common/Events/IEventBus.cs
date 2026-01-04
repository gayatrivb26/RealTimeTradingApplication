
namespace RealTimeTradingApp.Application.Common.Events
{
    /// <summary>
    /// Simple pub/sub event bus abstraction
    /// </summary>
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default)
            where TEvent: IIntegrationEvent;
    }
}
