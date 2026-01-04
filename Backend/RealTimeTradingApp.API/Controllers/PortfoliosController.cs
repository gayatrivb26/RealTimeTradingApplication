using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealTimeTradingApp.API.Filters;
using RealTimeTradingApp.Application.Features.Holdings.Queries;
using RealTimeTradingApp.Application.Features.Portfolios.Commands.CreatePortfolio;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;
using RealTimeTradingApp.Application.Features.Portfolios.Queries.GetPortfolioById;
using RealTimeTradingApp.Application.Features.Portfolios.Queries.GetPortfolioSummary;

namespace RealTimeTradingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomExceptionFilter]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PortfoliosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(PortfolioDto), 200)]
        [ProducesResponseType(typeof(string[]), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<PortfolioDto>> Create([FromBody] CreatePortfolioCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PortfolioDto), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<PortfolioDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPortfolioByIdQuery { Id = id });
            return Ok(result);
        }

        [HttpGet("{id}/holdings")]
        public async Task<IActionResult> GetHoldings(Guid id)
        {
            var response = await _mediator.Send(new GetPortfolioHoldingsQuery { PortfolioId = id });
            return Ok(response);
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetSummary(Guid id)
        {
            var response = await _mediator.Send(new GetPortfolioSummaryQuery { PortfolioId = id });
            return Ok(response);
        }
    }
}
