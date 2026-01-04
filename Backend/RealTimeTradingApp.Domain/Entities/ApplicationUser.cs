
using Microsoft.AspNetCore.Identity;

namespace RealTimeTradingApp.Domain.Entities
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public decimal Balance { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}
