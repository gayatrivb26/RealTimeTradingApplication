using MediatR;
using RealTimeTradingApp.Application.Features.Users.Dtos;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<UserDto> //returns userID
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public decimal InitialBalance { get; set; }
        public string Password { get; set; } = default!;
    }
}
