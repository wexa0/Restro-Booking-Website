using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Resort.Domain.Users;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> FindByIdAsync(string id, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}
