
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Common.Models;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TradingDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            TradingDbContext context,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string ip)
         {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.CreateAccessToken(user, roles);
            var refreshToken = _tokenService.CreateRefreshToken(ip);

            refreshToken.UserId = user.Id;

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse(accessToken, refreshToken.Token);
        }

        public async Task<AuthResponse> RefreshAsync(string token, string ip)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token &&
                                          r.RevokedAt == null &&
                                          r.ExpiresAt > DateTime.UtcNow)
                ?? throw new UnauthorizedAccessException("Invalid refresh token");

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ip;

            var newRefreshToken = _tokenService.CreateRefreshToken(ip);
            newRefreshToken.UserId = refreshToken.UserId;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            var roles = await _userManager.GetRolesAsync(refreshToken.User);

            var accessToken = _tokenService.CreateAccessToken(refreshToken.User, roles);

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse(accessToken, newRefreshToken.Token);
        }

        public async Task LogoutAsync(string token, string ip)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token &&
                                          r.RevokedAt == null &&
                                          r.ExpiresAt > DateTime.UtcNow);

            if (refreshToken == null) return;

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ip;

            await _context.SaveChangesAsync();
        }
    }
}
