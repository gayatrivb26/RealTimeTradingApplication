using FluentValidation;


namespace RealTimeTradingApp.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderValidator: AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
