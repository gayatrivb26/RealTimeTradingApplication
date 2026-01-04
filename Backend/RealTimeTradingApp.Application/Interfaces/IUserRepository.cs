using RealTimeTradingApp.Domain.Entities;

namespace RealTimeTradingApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<int> GetCountAsync(CancellationToken cancellationToken);
        Task<User> AddAsync(User user, CancellationToken cancellationToken);
    }
}
