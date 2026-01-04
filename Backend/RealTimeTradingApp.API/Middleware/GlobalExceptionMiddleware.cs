using FluentValidation;
using RealTimeTradingApp.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace RealTimeTradingApp.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                (HttpStatusCode statusCode, object message) result;

                switch(ex)
                {
                    case  CustomValidationException validationException:
                        result = (HttpStatusCode.BadRequest, (object)validationException.Errors);
                        break;
                    
                    case UnauthorizedException:
                        result = (HttpStatusCode.Unauthorized, "Unauthorized access");
                        break;

                    case NotFoundException:
                        result = (HttpStatusCode.NotFound, "Resource not found");
                        break;

                    case ForbiddenException:
                        result = (HttpStatusCode.Forbidden, "Forbidden Access");
                        break;

                    default:
                        result = (HttpStatusCode.InternalServerError, "An unexpected error occured.");
                        break;
                }
                var (statusCode, message) = result;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
                var json = JsonSerializer.Serialize(message);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
