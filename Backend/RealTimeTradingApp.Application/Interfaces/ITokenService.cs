
using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(ApplicationUser user, IList<string> roles);
        RefreshToken CreateRefreshToken(string ipAddress);
    }
}
