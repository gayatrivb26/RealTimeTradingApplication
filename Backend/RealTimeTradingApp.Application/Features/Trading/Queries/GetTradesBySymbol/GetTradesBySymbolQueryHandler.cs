
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Trading.Dtos;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradesBySymbol
{
    public class GetTradesBySymbolQueryHandler: IRequestHandler<GetTradesBySymbolQuery, List<TradeDto>>
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMapper _mapper;
        private readonly IPortfolioRepository _portfolioRepository;

        public GetTradesBySymbolQueryHandler(
            ITradeRepository tradeRepository,
            IMapper mapper,
            IPortfolioRepository portfolioRepository)
        {
            _tradeRepository = tradeRepository;
            _mapper = mapper;
            _portfolioRepository = portfolioRepository;
        }

        public async Task<List<TradeDto>> Handle(GetTradesBySymbolQuery request, CancellationToken ct)
        {
            if (request.PortfolioId.HasValue)
            {
                var portfolio = await _portfolioRepository.GetByIdAsync(request.PortfolioId.Value, ct);

                if (portfolio == null)
                    throw new NotFoundException($"Portfolio with id '{request.PortfolioId}' not found.");
            }

            var query = _tradeRepository.Query()
                .Where(t => t.Symbol == request.Symbol);

            if (request.PortfolioId.HasValue)
                query = query.Where(t => t.PortfolioId == request.PortfolioId.Value);

            var trades = await query
                .OrderBy(t => t.ExecutedAt)
                .ToListAsync(ct);

            return _mapper.Map<List<TradeDto>>(trades);
        }
    }
}
