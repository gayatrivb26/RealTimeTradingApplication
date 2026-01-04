using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealTimeTradingApp.API.Filters;
using RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeById;
using RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeHistory;
using RealTimeTradingApp.Application.Features.Trading.Queries.GetTradesBySymbol;
using RealTimeTradingApp.Application.Features.Trading.Queries.GetTradeSummary;

namespace RealTimeTradingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomExceptionFilter]
    public class TradesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TradesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeById(Guid id)
        {
            var result = await _mediator.Send(new GetTradeByIdQuery { TradeId = id });
            return Ok(result);
        }

        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary([FromQuery] GetTradeSummaryQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Symbol")]
        public async Task<IActionResult> GetTradesBySymbol([FromQuery] GetTradesBySymbolQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("History")]
        public async Task<IActionResult> GetTradeHistory([FromQuery] GetTradeHistoryQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
