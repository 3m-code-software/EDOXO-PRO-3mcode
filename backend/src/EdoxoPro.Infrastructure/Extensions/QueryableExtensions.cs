using System.Linq.Expressions;

namespace EdoxoPro.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(param, sortBy);
        var lambda = Expression.Lambda(property, param);

        var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var methodName = isDescending ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.Type],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
}
