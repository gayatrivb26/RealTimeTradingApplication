using MediatR;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Users.Dtos;

namespace RealTimeTradingApp.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery: IRequest<PagedResult<UserDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
