
namespace RealTimeTradingApp.Infrastructure.Auth
{
    public class JwtOptions
    {
        public string Key { get; set; } = default!;
        public int AccessTokenMinutes { get; set; }
        public int RefreshTokenDays { get; set; }
    }
}
