using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Features.Users.Dtos;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler: IRequestHandler<GetAllUsersQuery, PagedResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {

            var query = _userManager.Users.AsNoTracking();
            
            var totalCount = await query.CountAsync(cancellationToken);

            //var users = await _userRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

            var users = await query
                .OrderBy(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(users);

                return new PagedResult<UserDto>
                {
                    Items = userDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            
        }
    }
}
