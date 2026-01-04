using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Users.Dtos;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            
                var user = await _userManager.FindByIdAsync(request.Id.ToString()); ;

                if (user == null)
                {
                    throw new NotFoundException($"User with ID '{request.Id}' not found.");
                }

                return _mapper.Map<UserDto>(user);
        }
    }
}
