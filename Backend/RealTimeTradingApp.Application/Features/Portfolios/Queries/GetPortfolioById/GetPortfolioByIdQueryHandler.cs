using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;


namespace RealTimeTradingApp.Application.Features.Portfolios.Queries.GetPortfolioById
{
    public class GetPortfolioByIdQueryHandler: IRequestHandler<GetPortfolioByIdQuery, PortfolioDto> {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IStockPriceService _stockPriceService;
        private readonly IMapper _mapper;

        public GetPortfolioByIdQueryHandler(IPortfolioRepository portfolioRepository, IStockPriceService stockPriceService, IMapper mapper)
        {
            _portfolioRepository = portfolioRepository;
            _stockPriceService = stockPriceService;
            _mapper = mapper;
        }

        public async Task<PortfolioDto> Handle(GetPortfolioByIdQuery request, CancellationToken cancellationToken)
        {
            
                var portfolio = await _portfolioRepository.GetByIdAsync(request.Id, cancellationToken);
                if (portfolio == null)
                    throw new NotFoundException($"Portfolio with ID '{request.Id}' not found.");

                // Update stock prices from the Redis cache
                foreach (var holding in portfolio.Holdings)
                {
                    var livePrice = await _stockPriceService.GetLivePriceAsync(holding.Stock.Symbol, cancellationToken);

                    if (livePrice.HasValue)
                        holding.Stock.CurrentPrice = livePrice.Value;
                }

                // Simplified XIRR 
                var xirr = CalculateXIRR(portfolio.Holdings);
                var dto = _mapper.Map<PortfolioDto>(portfolio);
                dto.XIRR = xirr;

                return dto;
           
        }

        private decimal CalculateXIRR(ICollection<Holding> holdings)
        {
            return holdings.Any() ? holdings.Average(h => h.UnrealizedPnL / (h.Quantity * h.AverageCost)) * 100 : 0;
        }
    }
}
