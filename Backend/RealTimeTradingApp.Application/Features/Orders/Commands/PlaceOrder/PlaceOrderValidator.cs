using FluentValidation;
using RealTimeTradingApp.Application.Features.Orders.Commands.PlaceOrder;
using System;

namespace RealTimeTradingApp.Application.Features.Orders.Commands
{
    public class PlaceOrderValidator: AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.PortfolioId)
                .NotEmpty().WithMessage("PortfolioId is required.");

            RuleFor(x => x.symbol)
                .NotEmpty().WithMessage("Symbol is required.")
                .MaximumLength(50);

            RuleFor(x => x.Side)
                .NotEmpty().WithMessage("Side is required.")
                .Must(s => s == "Buy" || s == "Sell")
                .WithMessage("Side must be buy or sell.");

            RuleFor(x => x.OrderType)
                .NotEmpty().WithMessage("OrderType is required.")
                .Must(s => s == "Market" || s == "Limit")
                .WithMessage("OrderType must be 'Market' or 'Limit'.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            When(x => x.OrderType == "Limit", () =>
            {
                RuleFor(x => x.LimitPrice)
                    .NotNull().WithMessage("Limit price is required for Limit orders.")
                    .GreaterThan(0).WithMessage("Limit price must be greater than zero.");
            });
        }
    }
}
