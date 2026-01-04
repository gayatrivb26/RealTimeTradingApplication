
namespace RealTimeTradingApp.Domain.Entities
{
    public class Portfolio
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid UserId { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = default!;
        public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
