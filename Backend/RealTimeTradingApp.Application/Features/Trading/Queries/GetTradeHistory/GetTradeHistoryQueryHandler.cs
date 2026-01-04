using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Trading.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using System.Linq;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeHistory
{
    public class GetTradeHistoryQueryHandler: IRequestHandler<GetTradeHistoryQuery, PagedResult<TradeDto>>
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMapper _mapper;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetTradeHistoryQueryHandler(
            ITradeRepository tradeRepository,
            IMapper mapper,
            IPortfolioRepository portfolioRepository,
            UserManager<ApplicationUser> userManager)
        {
            _tradeRepository = tradeRepository;
            _mapper = mapper;
            _portfolioRepository = portfolioRepository;
            _userManager = userManager;
        }

        public async Task<PagedResult<TradeDto>> Handle(GetTradeHistoryQuery request, CancellationToken ct)
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

            var query = _tradeRepository.Query(); // Expose IQueryable<Trade>

            if (request.PortfolioId.HasValue)
                query =  query.Where(t => t.PortfolioId == request.PortfolioId.Value);

            if (request.UserId.HasValue)
                query = query.Where(t => t.Portfolio.UserId == request.UserId.Value);

            if (!string.IsNullOrWhiteSpace(request.Symbol))
                query = query.Where(t => t.Symbol == request.Symbol);

            if (request.From.HasValue)
                query = query.Where(t => t.ExecutedAt >= request.From.Value);

            if(request.To.HasValue)
                query = query.Where(t => t.ExecutedAt <= request.To.Value);

            query = query.OrderByDescending(t => t.ExecutedAt);

            var total = await query.CountAsync(ct);

            var trades = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            var dtos = _mapper.Map<List<TradeDto>>(trades);

            return new PagedResult<TradeDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total
            };
        }
    }
}
