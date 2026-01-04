
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RealTimeTradingApp.Application.Common.Behaviors;
using RealTimeTradingApp.Application.Common.Events;
using RealTimeTradingApp.Application.Features.Trading.Services;
using RealTimeTradingApp.Application.Features.Users.Commands.CreateUser;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Application.Mappings;


namespace RealTimeTradingApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplcationServices(this IServiceCollection services)
        {
            // Add MediatR and Automapper 
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

            // Add FluentValidation
            services.AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            
            services.AddScoped<IMachingEngineService, MatchingEngineService>();
            
            return services;
        }
    }
}
