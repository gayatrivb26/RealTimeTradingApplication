using MediatR;
using RealTimeTradingApp.Application.Features.Users.Dtos;

namespace RealTimeTradingApp.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid Id { get; set; }
    }
}
