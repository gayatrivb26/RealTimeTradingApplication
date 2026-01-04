using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Users.Dtos;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            
                var existingUser = await _userManager.FindByEmailAsync(request.Email);

                if (existingUser != null)
                    throw new CustomValidationException("Email address already exists."); ;

                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Balance = request.InitialBalance,
                    CreatedAt = DateTime.UtcNow
                };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new CustomValidationException(
                    string.Join(",", result.Errors.Select((e => e.Description))));

            await _userManager.AddToRoleAsync(user, "Trader");

            return _mapper.Map<UserDto>(user);
        }
    }
}
