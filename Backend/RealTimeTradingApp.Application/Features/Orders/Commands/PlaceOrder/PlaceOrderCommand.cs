using MediatR;
using RealTimeTradingApp.Application.Features.Orders.Dtos;

namespace RealTimeTradingApp.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommand: IRequest<OrderDto>
    {
        public Guid UserId { get; set; }
        public Guid PortfolioId { get; set; }

        public string symbol { get; set; } = default!;

        public string Side { get; set; } = default!;
        public string OrderType { get; set; } = default!;

        public decimal Quantity {  get; set; }
        public decimal? LimitPrice { get; set; }

    }
}
