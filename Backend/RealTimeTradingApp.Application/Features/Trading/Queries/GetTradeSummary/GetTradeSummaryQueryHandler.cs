
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Trading.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeSummary
{
    internal class GetTradeSummaryQueryHandler: IRequestHandler<GetTradeSummaryQuery, TradeSummaryDto>
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetTradeSummaryQueryHandler(ITradeRepository tradeRepository, IPortfolioRepository portfolioRepository, UserManager<ApplicationUser> userManager)
        {
            _tradeRepository = tradeRepository;
            _portfolioRepository = portfolioRepository;
            _userManager = userManager;
        }

        public async Task<TradeSummaryDto> Handle(GetTradeSummaryQuery request, CancellationToken ct)
        {
            if (request.UserId.HasValue)
            {
                var userExists = await _userManager.FindByIdAsync(request.UserId.Value.ToString());

                if (userExists == null)
                    throw new NotFoundException($"User with id '{request.UserId}' not found.");
            }

            if (request.PortfolioId.HasValue)
            {
                var portfolio = await _portfolioRepository.GetByIdAsync(request.PortfolioId.Value, ct);

                if (portfolio == null)
                    throw new NotFoundException($"Portfolio with id '{request.PortfolioId}' not found.");

                if (request.UserId.HasValue && portfolio.UserId != request.UserId)
                    throw new ForbiddenException("Portfolio with id '{request.PortfolioId}' not found.");
            }

            var query = _tradeRepository.Query();

            if (request.PortfolioId.HasValue)
                query = query.Where(t => t.PortfolioId == request.PortfolioId);

            if (request.UserId.HasValue)
                query = query.Where(t => t.Portfolio.UserId == request.UserId);

            var totalTrades = await query.CountAsync(ct);
            var totalVolume = await query.SumAsync(t => t.Quantity, ct);

            var totalBuy = await query.Where(t => t.BuyOdrderId != Guid.Empty).SumAsync(t => t.Quantity, ct);
            var totalSell = await query.Where(t => t.SellOdrderId != Guid.Empty).SumAsync(t => t.Quantity, ct);

            // Realized P&L simplified: SellPrice - BuyPrice times qty
            var realizedPnL = await query.SumAsync(t => (t.Price * t.Quantity), ct);

            return new TradeSummaryDto
            {
                TotalTrades = totalTrades,
                TotalVolume = totalVolume,
                TotalBuyVolume = totalBuy,
                TotalSellVolume = totalSell,
                RealizedPnL = realizedPnL
            };
        }
    }
}
