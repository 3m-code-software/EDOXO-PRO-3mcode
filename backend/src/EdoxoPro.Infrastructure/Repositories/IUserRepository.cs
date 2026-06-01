using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Infrastructure.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserWithRolesAsync(int userId);
    Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName);
    Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm);
}
