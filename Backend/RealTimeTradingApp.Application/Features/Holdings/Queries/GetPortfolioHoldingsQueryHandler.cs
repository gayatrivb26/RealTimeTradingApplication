using AutoMapper;
using MediatR;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Holdings.Dtos;
using RealTimeTradingApp.Application.Interfaces;


namespace RealTimeTradingApp.Application.Features.Holdings.Queries
{
    public class GetPortfolioHoldingsQueryHandler: IRequestHandler<GetPortfolioHoldingsQuery, List<HoldingDto>>
    {
        private readonly IHoldingRepository _holdingRepository;
        private readonly IMapper _mapper;

        public GetPortfolioHoldingsQueryHandler(IHoldingRepository holdingRepository, IMapper mapper)
        {
            _holdingRepository = holdingRepository;
            _mapper = mapper;
        }

        public async Task<List<HoldingDto>> Handle(GetPortfolioHoldingsQuery request, CancellationToken ct)
        {
            var holdings = await _holdingRepository.GetHoldingsForPortfolioAsync(request.PortfolioId, ct);

            if (holdings.Count == 0)
                throw new NotFoundException($"Portfolio with ID '{request.PortfolioId}' not found.");

            return holdings.Select(h => new HoldingDto
            {
                Symbol = h.Stock.Symbol,
                Quantity = h.Quantity,
                AvgCost = h.AverageCost,
                CurrentPrice = h.Stock.CurrentPrice,
                CurrentValue = h.Quantity * h.Stock.CurrentPrice,
                UnrealizedPnL = (h.Stock.CurrentPrice - h.AverageCost) * h.Quantity,
                PnLPercentage = h.AverageCost > 0 ? ((h.Stock.CurrentPrice - h.AverageCost) / h.AverageCost) * 100 : 0,
                Sector = h.Stock.Sector ?? "Unknown"
            }).ToList();
        }
    }
}
