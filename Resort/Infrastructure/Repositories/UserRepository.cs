using Microsoft.EntityFrameworkCore;
using Resort.Domain.Users;
using Resort.Infrastructure.Database;

namespace Resort.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<List<User>> GetAllAsync(CancellationToken ct = default)
        => _db.Users.AsNoTracking().ToListAsync(ct);

    public Task<User?> FindByIdAsync(string id, CancellationToken ct = default)
        => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default)
        => _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }
}
