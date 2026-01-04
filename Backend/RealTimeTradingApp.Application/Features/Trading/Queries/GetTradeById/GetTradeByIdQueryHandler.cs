

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Trading.Dtos;
using RealTimeTradingApp.Application.Interfaces;

namespace RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeById
{
    public class GetTradeByIdQueryHandler: IRequestHandler<GetTradeByIdQuery, TradeDto>
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMapper _mapper;

        public GetTradeByIdQueryHandler(
            ITradeRepository tradeRepository,
            IMapper mapper)
        {
            _tradeRepository = tradeRepository;
            _mapper = mapper;
        }

        public async Task<TradeDto> Handle(GetTradeByIdQuery request, CancellationToken ct)
        {
            var trade = await _tradeRepository.Query()
                .FirstOrDefaultAsync(t => t.Id == request.TradeId, ct);

            if (trade == null)
                throw new NotFoundException($"Trade with ID {request.TradeId} not found.");

            return _mapper.Map<TradeDto>(trade);
        }
    }
}
