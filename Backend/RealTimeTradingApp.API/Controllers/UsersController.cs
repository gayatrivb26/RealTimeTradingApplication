using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealTimeTradingApp.API.Filters;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Users.Commands.CreateUser;
using RealTimeTradingApp.Application.Features.Users.Dtos;
using RealTimeTradingApp.Application.Features.Users.Queries.GetAllUsers;
using RealTimeTradingApp.Application.Features.Users.Queries.GetUserById;
using System.Text.Json;

namespace RealTimeTradingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [CustomExceptionFilter]
    public class UsersController: ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(string[]), 400)]
        public async Task<ActionResult<UserDto>> Create(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var userDto = await _mediator.Send(command);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(string), 404)]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var userDto = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(userDto);
        }

        ///// <summary>
        ///// Gets a paginated list of users.
        ///// </summary>
        ///// <param name="page">Page number (default: 1).</param>
        ///// <param name="pageSize">Items per page (default: 10).</param>
        /////// <param name="includePortfolios">Whether to include portfolio details.</param>
        ///// <returns>A paginated list of users.</returns>
        ///// <response code="200">Returns the paginated user list.</response>
        ///// <response code="400">If pagination parameters are invalid.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserDto>), 200)]
        [ProducesResponseType(typeof(string[]), 400)]
        public async Task<ActionResult<UserDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllUsersQuery { Page = page, PageSize = pageSize });
            Console.WriteLine($"GetAll Response: {JsonSerializer.Serialize(result)}"); // Debug
            return Ok(result);
        }
    }
}
