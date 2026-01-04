
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealTimeTradingApp.Application.Common.Events;
using RealTimeTradingApp.Application.Features.Trading.Handlers;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Infrastructure.Auth;
using RealTimeTradingApp.Infrastructure.Data;
using RealTimeTradingApp.Infrastructure.Events;
using RealTimeTradingApp.Infrastructure.Repositories;
using RealTimeTradingApp.Infrastructure.Services;

namespace RealTimeTradingApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            // Add Context
            services.AddDbContext<TradingDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ITradeRepository, TradeRepository>();
            services.AddScoped<IHoldingRepository, HoldingRepository>();
            services.AddScoped<IStockRepository, StockRepository>();

            services.AddScoped<IStockPriceService, StockPriceService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddStackExchangeRedisCache(options =>
            options.Configuration = config.GetConnectionString("Redis"));

            services.AddScoped<ICacheService, RedisCacheService>();


            // Event Bus (In-memory)
            services.AddSingleton<IEventBus, InMemoryEventBus>();

            //Register event handlers
            services.AddScoped<IIntegrationEventHandler<OrderPlacedEvent>, OrderPlacedEventHandler>();


            //services.AddHostedService<MarketDataService>();

            return services;
        }
    }
}
