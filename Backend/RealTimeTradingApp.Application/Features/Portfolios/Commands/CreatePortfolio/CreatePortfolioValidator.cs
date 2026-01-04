using FluentValidation;

namespace RealTimeTradingApp.Application.Features.Portfolios.Commands.CreatePortfolio
{
    public class CreatePortfolioValidator : AbstractValidator<CreatePortfolioCommand>
    {
        public CreatePortfolioValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
