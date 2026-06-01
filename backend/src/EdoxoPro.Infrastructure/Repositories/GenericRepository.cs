using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EdoxoPro.Application.Common;
using EdoxoPro.Application.Interfaces;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Infrastructure.Data;
using System.Linq.Dynamic.Core;

namespace EdoxoPro.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();
        return await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void SoftDelete(BaseEntity entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity as T ?? throw new InvalidOperationException("Entity must be of type BaseEntity"));
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<EdoxoPro.Application.Common.PagedResult<T>> GetPagedAsync(FilterRequest request)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(GetSearchPredicate(request.Search));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var direction = request.SortDirection == "desc" ? "descending" : "ascending";
            query = query.OrderBy($"{request.SortBy} {direction}");
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new EdoxoPro.Application.Common.PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static System.Linq.Expressions.Expression<Func<T, bool>> GetSearchPredicate(string search)
    {
        var param = System.Linq.Expressions.Expression.Parameter(typeof(T), "x");
        var properties = typeof(T).GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanRead)
            .ToList();

        if (properties.Count == 0)
            return x => true;

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        if (toLowerMethod == null || containsMethod == null)
            return x => true;

        var lowerSearch = System.Linq.Expressions.Expression.Constant(search.ToLower());
        var body = properties.Select(prop =>
        {
            var propExpr = System.Linq.Expressions.Expression.Property(param, prop);
            var propNotNull = System.Linq.Expressions.Expression.NotEqual(propExpr, System.Linq.Expressions.Expression.Constant(null));
            var propLower = System.Linq.Expressions.Expression.Call(propExpr, toLowerMethod);
            var containsExpr = System.Linq.Expressions.Expression.Call(propLower, containsMethod, lowerSearch);
            return System.Linq.Expressions.Expression.AndAlso(propNotNull, containsExpr);
        }).Aggregate(System.Linq.Expressions.Expression.OrElse);

        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, param);
    }
}
