using Microsoft.Extensions.Logging;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Features.Trading.Services
{
    public class MatchingEngineService: IMachingEngineService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITradeRepository _tradeRepository;
        private readonly ILogger<MatchingEngineService> _logger;

        public MatchingEngineService(
            IOrderRepository orderRepository,
            ITradeRepository tradeRepository,
            ILogger<MatchingEngineService> logger)
        {
            _orderRepository = orderRepository;
            _tradeRepository = tradeRepository;
            _logger = logger;
        }
        
        public async Task<IReadOnlyList<Trade>> MatchAsync(Order incomingOrder, CancellationToken ct)
        {
            var trades = new List<Trade>();

            if(incomingOrder == null )
                throw new ArgumentNullException(nameof(incomingOrder));

            if (incomingOrder.RemainingQuantity <= 0)
                return trades;

            _logger.LogInformation($"Matching incoming order {incomingOrder.Id} {incomingOrder.Side} " +
                $"{incomingOrder.RemainingQuantity}@{incomingOrder.Price} for {incomingOrder.Symbol}");

            var oppositeSide = incomingOrder.Side == OrderSide.Buy
                ? OrderSide.Sell : OrderSide.Buy;

            // 1. Load opposite-side open orders for this symbol
            var bookOrders = await _orderRepository.GetOpenOrdersForMatchingAsync(
                incomingOrder.Symbol, oppositeSide, ct);

            // 2. Filter by price condition (if limit order)
            bookOrders = FilterByPrice(incomingOrder, bookOrders);

            // 3. Apply price-time priority sorting
            bookOrders = SortByPriceTimePriority(incomingOrder, bookOrders);

            // 4. Iterate and match
            foreach (var bookOrder in bookOrders)
            {
                if (incomingOrder.RemainingQuantity <= 0)
                    break;

                if (bookOrder.RemainingQuantity <= 0)
                    continue; // skip already filled/corrupt orders

                // Determine trade quantity
                var tradeQty = Math.Min(incomingOrder.RemainingQuantity, bookOrder.RemainingQuantity);

                // Trade price: typically resting order's price
                var tradePrice = bookOrder.Price;

                var trade = new Trade
                {
                    Id = Guid.NewGuid(),
                    Symbol = incomingOrder.Symbol,
                    Quantity = tradeQty,
                    Price = tradePrice,
                    BuyOdrderId = incomingOrder.Side == OrderSide.Buy ? incomingOrder.Id : bookOrder.Id,
                    SellOdrderId = incomingOrder.Side == OrderSide.Sell ? incomingOrder.Id : bookOrder.Id,
                    ExecutedAt = DateTime.UtcNow
                };
                
                trades.Add(trade);

                _logger.LogInformation($"Matched {tradeQty} {incomingOrder.Symbol} at {tradePrice} " +
                    $"between orders {incomingOrder.Id} and {bookOrder.Id}");

                // Adjust quantities
                incomingOrder.RemainingQuantity -= tradeQty;
                bookOrder.RemainingQuantity -= tradeQty;

                UpdateOrderStatus(incomingOrder);
                UpdateOrderStatus(bookOrder);

                await _orderRepository.UpdateAsync(bookOrder, ct);
            }

            // Save trades
            if (trades.Any())
            {
                await _tradeRepository.AddRangeAsync(trades, ct);
            }

            // If limit order still has quantity, it stays open (already persisted before matching)
            // If market order has remaining qty, we usually cancel the remainder
            if(incomingOrder.RemainingQuantity > 0 && incomingOrder.Type == OrderType.Market)
            {
                incomingOrder.Status = OrderStatus.PartiallyFilled;
            }

            await _orderRepository.UpdateAsync(incomingOrder, ct);

            // Commit changes in a single transaction at repo layer
            await _orderRepository.SaveChangesAsync(ct);
            await _tradeRepository.SaveChangesAsync(ct);

            return trades;
        }

        /// <summary>
        /// Filters opposite orders by price condition based on incoming order type and side.
        /// </summary>
        /// <param name="incoming"></param>
        /// <param name="bookOrders"></param>
        /// <returns></returns>
        private List<Order> FilterByPrice(Order incoming, List<Order> bookOrders)
        {
            if(incoming.Type == OrderType.Market)
            {
                // Market order -> no price constraint (just best available
                return bookOrders;
            }

            // Limit orders apply constraints
            if(incoming.Side == OrderSide.Buy)
            {
                // Buy limit: can only match with sell orders at or below buy price
                return bookOrders
                    .Where(o => o.Price <= incoming.Price)
                    .ToList();
            }
            else
            {
                //Sell limit: can only match with buy orders at or above sell price
                return bookOrders
                    .Where(o => o.Price >= incoming.Price)
                    .ToList();
            }
        }

        private List<Order> SortByPriceTimePriority(Order incoming, List<Order> bookOrders)
        {
            if(incoming.Side == OrderSide.Buy)
            {
                // Buyer wants lowest price first
                return bookOrders
                    .OrderBy(o => o.Price)   // lowest price
                    .ThenBy(o => o.CreatedAt) // oldest order first
                    .ToList();
            }
            else
            {
                // Sellet wants highest price first
                return bookOrders
                    .OrderByDescending(o => o.Price) // highest price
                    .ThenBy(o => o.CreatedAt) // oldest order first
                    .ToList();
            }
        }

        private void UpdateOrderStatus(Order order)
        {
            if (order.RemainingQuantity == 0)
            {
                order.Status = OrderStatus.Filled;
            }
            else if (order.RemainingQuantity > 0 && order.RemainingQuantity < order.Quantity)
            {
                order.Status = OrderStatus.PartiallyFilled;
            }
            else
            {
                // Still full quantity left (not matched at all)
                order.Status = OrderStatus.New;
            }
        }
    }
}
