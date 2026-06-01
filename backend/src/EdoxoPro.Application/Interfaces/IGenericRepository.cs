using System.Linq.Expressions;
using EdoxoPro.Application.Common;
using EdoxoPro.Domain.Entities;

namespace EdoxoPro.Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SoftDelete(BaseEntity entity);
    Task SaveChangesAsync();
    IQueryable<T> GetQueryable();
    Task<PagedResult<T>> GetPagedAsync(FilterRequest request);
}
