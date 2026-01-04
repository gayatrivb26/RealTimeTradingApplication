
namespace RealTimeTradingApp.Application.Common.Events
{
    /// <summary>
    /// Contract for handling integration events.
    /// </summary>
    /// <typeparam name="TEvent"> Event type this handler processes.</typeparam>
    public interface IIntegrationEventHandler<in TEvent>
        where TEvent: IIntegrationEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken ct);        
    }
}
