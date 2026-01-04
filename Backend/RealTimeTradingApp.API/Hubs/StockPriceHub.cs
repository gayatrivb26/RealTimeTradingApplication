using Microsoft.AspNetCore.SignalR;

namespace RealTimeTradingApp.API.Hubs
{
    public class StockPriceHub: Hub
    {
        public async Task SubscribeToStock(string symbol)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"stock:{symbol}");
        }

        public async Task SubscribeToPortfolio(Guid portgolioId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"portfolio:{portgolioId}");
        }

        public async Task Unsubscribe(string group)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }
    }
}
