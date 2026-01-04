
using RealTimeTradingApp.Application.Common.Models;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, string ip);
        Task<AuthResponse> RefreshAsync(string refreshToken, string ip);
        Task LogoutAsync(string refreshToken, string ip);
    }
}
