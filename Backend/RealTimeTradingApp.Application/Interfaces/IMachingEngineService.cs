
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    /// <summary>
    /// Encapsulated the core matching logic for buy/sell orders.
    /// Takes a new orderand tried to match it against existing opposite orders.
    /// </summary>
    public interface IMachingEngineService
    {
        /// <summary>
        /// Attempts to match the given order with existing  orders.
        /// Returns the list of trades executed as a result.
        /// </summary>
        /// <param name="incomingOrder"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IReadOnlyList<Trade>> MatchAsync(Order incomingOrder, CancellationToken ct);
    }
}
