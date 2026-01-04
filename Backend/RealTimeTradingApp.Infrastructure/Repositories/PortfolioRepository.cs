using Microsoft.EntityFrameworkCore;
using RealTimeTradingApp.Application.Interfaces;
using RealTimeTradingApp.Domain.Entities;
using RealTimeTradingApp.Infrastructure.Data;

namespace RealTimeTradingApp.Infrastructure.Repositories
{
    public class PortfolioRepository: IPortfolioRepository
    {
        private readonly TradingDbContext _context;

        public PortfolioRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken cancellationToken)
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync(cancellationToken);

            return portfolio; ;
        }

        public async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Portfolios
                .AsNoTracking()
                .Include(u => u.User)
                .Include(p => p.Holdings)
                .ThenInclude(h => h.Stock)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Portfolio>>GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Portfolios
                .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByUserIdAndNameAsync(Guid userId, string name, CancellationToken cancellationToken)
        {
            return await _context.Portfolios
                .AnyAsync(p => p.UserId == userId && p.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
