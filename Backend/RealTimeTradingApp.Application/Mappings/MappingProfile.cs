using RealTimeTradingApp.Domain.Entities;
using AutoMapper;
using RealTimeTradingApp.Application.Features.Users.Dtos;
using RealTimeTradingApp.Application.Features.Portfolios.Dtos;
using RealTimeTradingApp.Application.Features.Holdings.Dtos;
using RealTimeTradingApp.Application.Features.Orders.Dtos;

namespace RealTimeTradingApp.Application.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Portfolio, PortfolioDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserBalance, opt => opt.MapFrom(src => src.User.Balance))
                .ForMember(dest => dest.TotalPortfolioValue, opt => opt.MapFrom(src => src.User.Balance + (src.Holdings.Any() ? src.Holdings.Sum(h => h.CurrentValue) : 0)))
                .ForMember(dest => dest.UnrealizedPnL, opt => opt.MapFrom(src => src.Holdings.Any() ? src.Holdings.Sum(h => h.UnrealizedPnL) : 0))
                .ForMember(dest => dest.Allocation, opt => opt.MapFrom(src => src.Holdings.Any() ? src.Holdings
                        .GroupBy(h => h.Sector)
                        .ToDictionary(g => g.Key, g => (decimal)g.Sum(h => h.CurrentValue) / src.Holdings.Sum(h => h.CurrentValue) * 100) : new Dictionary<string, decimal>()))
                .ForMember(dest => dest.Holdings, opt => opt.MapFrom(src => src.Holdings));

            CreateMap<Holding, HoldingDto>()
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Stock.Symbol))
                .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.Stock.CurrentPrice))
                .ForMember(dest => dest.CurrentValue, opt => opt.MapFrom(src => src.Quantity * src.Stock.CurrentPrice))
                .ForMember(dest => dest.UnrealizedPnL, opt => opt.MapFrom(src => src.Quantity * (src.Stock.CurrentPrice - src.AverageCost)));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Side, opt => opt.MapFrom(src => src.Side.ToString()))
                .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
