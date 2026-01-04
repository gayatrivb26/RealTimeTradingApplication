using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTimeTradingApp.Application.Common.Settings;
using System.Diagnostics;

namespace RealTimeTradingApp.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly PerformanceSettings _settings;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, IOptions<PerformanceSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();
            var response = await next();

            stopWatch.Stop();

            if(stopWatch.ElapsedMilliseconds > _settings.ThreshholdMs)
            {
                _logger.LogInformation("Long running quest {requestName} ({Elapsed} ms) Request: {@Request}",
                    typeof(TRequest).Name, stopWatch.ElapsedMilliseconds, request);
            }

            return response;
        }
    }
}
