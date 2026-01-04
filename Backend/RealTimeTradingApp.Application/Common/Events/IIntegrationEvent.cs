
namespace RealTimeTradingApp.Application.Common.Events
{
    public interface IIntegrationEvent
    {
        /// <summary>
        /// Base contract for integration events (things that happened in the system).
        /// These are immutable messages that can be published and consumed.
        /// </summary>
        Guid Id { get; }
        DateTime OccuredAt { get; }
    }
}
