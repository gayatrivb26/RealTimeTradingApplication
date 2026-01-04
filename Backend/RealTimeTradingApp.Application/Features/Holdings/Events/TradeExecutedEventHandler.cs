

using MediatR;
using Microsoft.Extensions.Logging;
using RealTimeTradingApp.Application.Common.Events;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Domain.Enums;

namespace RealTimeTradingApp.Application.Features.Holdings.Events
{
    /// <summary>
    /// Handles TradeExecutedEvent and updates portfolio holdings.
    /// </summary>
    public class TradeExecutedEventHandler: INotificationHandler<TradeExecutedEvent>
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IHoldingRepository _holdingRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<TradeExecutedEventHandler> _logger;

        public TradeExecutedEventHandler(
            IPortfolioRepository portfolioRepository,
            IHoldingRepository holdingRepository,
            IStockRepository stockRepository,
            ILogger<TradeExecutedEventHandler> logger)
        {
            _portfolioRepository = portfolioRepository;
            _holdingRepository = holdingRepository;
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task Handle(TradeExecutedEvent notification, CancellationToken ct)
        {
            _logger.LogInformation($"Processing holding update for the Trade {notification.Id} {notification.Symbol} Qty: {notification.Quantity} Price: {notification.Price}");

            // Ensure stock exists
            var stock = await _stockRepository.GetByIdAsync(notification.StockId, ct);
            if (stock == null)
            {
                _logger.LogInformation($"Stock {notification.StockId} not found when applying holding update.");
                return;
            }

            var holding = await _holdingRepository.GetByPortfolioAndStockAsync(notification.PortfolioId, notification.StockId, ct);

            if(holding == null)
            {
                // No position yet -> create new one
                holding = CreateNewHolding(notification, stock);
                await _holdingRepository.AddAsync(holding, ct);
            }
            else
            {
                ApplyTradeToExistingHolding(holding, notification);
                await _holdingRepository.UpdateAsync(holding, ct);
            }

            await _holdingRepository.SaveChangesAsync(ct);
        }

        private Holding CreateNewHolding(TradeExecutedEvent ev, Stock stock)
        {
            // For new position:
            // Buy -> Quantity = +Q, AvgCost = Price
            // Sell -> Quantity = -Q, AvgCost = PRice
            var signedQty = ev.Side == OrderSide.Buy
                ? ev.Quantity
                : -ev.Quantity;

            return new Holding
            {
                Id = Guid.NewGuid(),
                PortfolioId = ev.PortfolioId,
                StockId = ev.StockId,
                Quantity = signedQty,
                AverageCost = ev.Price,
                Sector = stock.Sector ?? "Unknown",
                Stock = stock
            };
        }

        private void ApplyTradeToExistingHolding(Holding holding, TradeExecutedEvent ev)
        {
            var currentQty = holding.Quantity;
            var avgCost = holding.AverageCost;
            var tradeQty = ev.Quantity;
            var tradePrice = ev.Price;
            var side = ev.Side;

            // CASE 1: No position
            if (currentQty == 0)
            {
                holding.Quantity = side == OrderSide.Buy ? tradeQty : -tradeQty;
                holding.AverageCost = tradePrice;
                return;
            }

            // LONG postion exists (CurrentQty > 0)
            if (currentQty > 0)
            {
                if (side == OrderSide.Buy)
                {
                    // Increase existing long
                    var newQty = currentQty + tradeQty;
                    var totalCost = (currentQty * avgCost) + (tradeQty * tradePrice);
                    holding.Quantity = newQty;
                    holding.AverageCost = totalCost / newQty;
                }
                else // side == sell
                {
                    if(tradeQty < currentQty)
                    {
                        // PArtial sell of long -> reduce quantity, keep avg cost
                        holding.Quantity = currentQty - tradeQty;
                        // AverageCost unchanges
                    }
                    else if (tradeQty == currentQty)
                    {
                        // Fully close ling position
                        holding.Quantity = 0;
                        holding.AverageCost = 0; 
                    }
                    else // tradeQty > currentQty -> flip from long -> short
                    {
                        var extraShort = tradeQty - currentQty;
                        holding.Quantity = -extraShort;
                        holding.AverageCost = tradePrice;
                    }
                }

                return;
            }

            // Short position exists
            var absCurrent = Math.Abs(currentQty);

            if(currentQty < 0)
            {
                if (side == OrderSide.Sell)
                {
                    // Increase short position
                    var newQtyAbs = absCurrent + tradeQty;
                    var totalCost = (absCurrent * avgCost) + (tradeQty * tradePrice);
                    holding.Quantity = -newQtyAbs;
                    holding.AverageCost = totalCost / newQtyAbs;
                }
                else // side == buy
                {
                    if(tradeQty < absCurrent)
                    {
                        // Partially cover short
                        holding.Quantity = -(absCurrent - tradeQty);
                    }
                    else if(tradeQty == absCurrent)
                    {
                        holding.Quantity = 0;
                        holding.AverageCost = 0;
                    }
                    else
                    {
                        var newLongQty = tradeQty - absCurrent;
                        holding.Quantity = newLongQty;
                        holding.AverageCost = tradePrice;
                    }
                }
            }
        }
    }
}
