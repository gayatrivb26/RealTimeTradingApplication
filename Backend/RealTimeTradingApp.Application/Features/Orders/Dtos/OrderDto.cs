
namespace RealTimeTradingApp.Application.Features.Orders.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PortfolioId { get; set; }

        public string Symbol { get; set; } = default!;

        public string Side { get; set; } = default!;
        public string OrderType { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }
        public string Status { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }
}
