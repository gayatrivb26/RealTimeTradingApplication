using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TradingDbContext _context;

        public UserRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<List<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.users
                .AsNoTracking()
                .OrderBy(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken)
        {
            return await _context.users.CountAsync(cancellationToken);
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken)
        {
            _context.users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
