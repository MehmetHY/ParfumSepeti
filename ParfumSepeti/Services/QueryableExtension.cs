using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ParfumSepeti.Services;

public static class QueryableExtension
{
    public static IQueryable<TEntity> Page<TEntity>(this IQueryable<TEntity> queryable,
                                                    int page,
                                                    int pageSize)
        => queryable.Skip((page - 1) * pageSize).Take(pageSize);

    public static IEnumerable<TEntity> Page<TEntity>(this IEnumerable<TEntity> entities,
                                                     int page,
                                                     int pageSize)
        => entities.Skip((page - 1) * pageSize).Take(pageSize);

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
}
