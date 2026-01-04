using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RealTimeTradingApp.Application.Common.Exceptions;
using RealTimeTradingApp.Application.Features.Holdings.Dtos;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Features.Portfolios.Commands.CreatePortfolio
{
    public class CreatePortfolioCommandHandler : IRequestHandler<CreatePortfolioCommand, PortfolioDto>
    {
        private readonly IPortfolioRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CreatePortfolioCommandHandler(IPortfolioRepository repository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PortfolioDto> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
        {
            
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                    throw new NotFoundException($"User ID with {request.UserId} not found.");

                var duplicate = await _repository.ExistsByUserIdAndNameAsync(request.UserId, request.Name, cancellationToken);
                if (duplicate)
                    throw new CustomValidationException($"Portfolio {request.Name} already exists for this user.");

                var portfolio = new Portfolio
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    UserId = request.UserId
                };

                var result = await _repository.AddAsync(portfolio, cancellationToken);

                var dto = _mapper.Map<PortfolioDto>(result);

                dto.UserFullName = $"{user.FirstName} {user.LastName}";
                dto.UserBalance = user.Balance;
                dto.TotalPortfolioValue = 0;dto.UnrealizedPnL = 0;
                dto.XIRR = 0;
                dto.Allocation = new Dictionary<string, decimal>();
                dto.Holdings = new List<HoldingDto>();

                return dto;
         
           
        }
    }
}
