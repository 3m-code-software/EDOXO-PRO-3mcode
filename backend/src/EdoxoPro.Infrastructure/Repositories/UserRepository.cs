using Microsoft.EntityFrameworkCore;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Infrastructure.Data;

namespace EdoxoPro.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetUserWithRolesAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName)
    {
        return await _dbSet
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.Roles.Any(ur => ur.Role.Name == roleName))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var term = searchTerm.Trim().ToLower();
        return await _dbSet
            .Where(u => u.Username.ToLower().Contains(term) ||
                        u.Email.ToLower().Contains(term) ||
                        u.FullName.ToLower().Contains(term) ||
                        (u.Phone != null && u.Phone.Contains(term)))
            .ToListAsync();
    }
}
