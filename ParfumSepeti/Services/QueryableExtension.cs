using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace ParfumSepeti.Services;

public static class QueryableExtension
{
    public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> queryable,
                                                    int page,
                                                    int pageSize)
        => queryable.Skip((page - 1) * pageSize).Take(pageSize);

    public static List<TEntity> Page<TEntity>(this IEnumerable<TEntity> entities,
                                                     int page,
                                                     int pageSize)
        => entities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    public static async Task<bool> ValidPageAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        int page,
        int pageSize
    )
    {
        if (page < 1 || pageSize < 1)
            return false;

        var count = await queryable.CountAsync();
        var pageCount = count / pageSize;

        if (pageCount == 0 || count % pageSize != 0)
            ++pageCount;

        return page <= pageCount;
    }

    public static bool ValidPage<TEntity>(this IEnumerable<TEntity> entities,
                                          int page,
                                          int pageSize)
    {
        if (page < 1 || pageSize < 1)
            return false;

        var count = entities.Count();
        var pageCount = count / pageSize;

        if (pageCount == 0 || count % pageSize != 0)
            ++pageCount;

        return page <= pageCount;
    }

    public static async Task<int> PageCountAsync<TEntity>(
        this IQueryable<TEntity> query,
        int pageSize
    )
    {
        var entityCount = await query.CountAsync();
        var pageCount = entityCount / pageSize;

        if (pageCount == 0 || entityCount % pageSize != 0)
            ++pageCount;

        return pageCount;
    }

    public static int PageCount<TEntity>(
        this IEnumerable<TEntity> entities,
        int pageSize
    )
    {
        var entityCount = entities.Count();
        var pageCount = entityCount / pageSize;

        if (pageCount == 0 || entityCount % pageSize != 0)
            ++pageCount;

        return pageCount;
    }

    public static TResult? Reduce<TEntity, TResult>(
        this IEnumerable<TEntity> entities,
        Func<TEntity, TResult?, TResult?> callback,
        TResult? initial = default)
    {
        foreach (var entity in entities)
            initial = callback(entity, initial);

        return initial;
    }
}
