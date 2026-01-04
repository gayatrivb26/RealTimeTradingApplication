using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RealTimeTradingApp.Application.Common.Exceptions;

namespace RealTimeTradingApp.API.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            object errorReponse = context.Exception switch
            {
                ValidationException validationEx => validationEx.Errors.Select(e => e.ErrorMessage).ToList(),
                CustomValidationException customEx => customEx.Errors.Count > 1 ? customEx.Errors : customEx.Errors.First(),
                NotFoundException notFoundEx => notFoundEx.Message,
                _ => "An unexpected error occured."
            };

            context.Result = new ObjectResult(errorReponse)
            {
                StatusCode = context.Exception switch
                {
                    ValidationException or CustomValidationException => 400,
                    NotFoundException => 404,
                    _ => 500
                }
            };

            context.ExceptionHandled = true;
        }
    }
}
