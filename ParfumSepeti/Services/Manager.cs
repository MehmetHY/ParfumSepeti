using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Data;
using System.Linq.Expressions;

namespace ParfumSepeti.Services;

public abstract class Manager<TEntity> where TEntity : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<TEntity> _set;

    public Manager(AppDbContext db)
    {
        _db = db;
        _set = _db.Set<TEntity>();
    }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public DbSet<TEntity> Set => _set;

    public IQueryable<TEntity> GetQueryable<TProperty>(
        IQueryable<TEntity>? queryable,
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, TProperty>>? include = null,
        bool tracked = true,
        int pageSize = 0,
        int page = 1)
    {
        if (queryable == null)
            queryable = _set;

        if (filter != null)
            queryable = queryable.Where(filter);

        if (!tracked)
            queryable = queryable.AsNoTracking();

        if (include != null)
            queryable = queryable.Include(include);

        if (pageSize > 0)
            queryable = queryable.Skip((page - 1) * pageSize).Take(pageSize);

        return queryable;
    }

    public IQueryable<TEntity> GetQueryable<TProperty>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, TProperty>>? include = null,
        bool tracked = true,
        int pageSize = 0,
        int page = 1
    )
    {
        IQueryable<TEntity> queryable = _set;

        return GetQueryable(queryable, filter, include, tracked, pageSize, page);
    }


    public IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? filter = null,
        bool tracked = true,
        int pageSize = 0,
        int page = 1
    )
        => GetQueryable<object>(filter, null, tracked, pageSize, page);


    public async Task<IEnumerable<TEntity>> GetAllAsync<TProperty>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, TProperty>>? include = null,
        bool tracked = true,
        int pageSize = 0,
        int page = 1
    )
    {
        var queryable = GetQueryable(filter, include, tracked, pageSize, page);

        return await queryable.ToListAsync();
    }


    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        bool tracked = true,
        int pageSize = 0,
        int page = 1
    )
        => await GetAllAsync<object>(filter, null, tracked, pageSize, page);


    public async Task<TEntity?> GetFirstOrDefaultAsync<TProperty>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TProperty>>? include = null,
        bool tracked = true
    )
        => await GetQueryable(filter, include, tracked).FirstOrDefaultAsync();
    
    public async Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> filter,
        bool tracked = true
    )
        => await GetQueryable(filter, tracked).FirstOrDefaultAsync();


    public async Task AddAsync(TEntity entity)
    {
        await _set.AddAsync(entity);
    }


    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _set.AddRangeAsync(entities);
    }


    public async Task AddRangeAsync(params TEntity[] entities)
    {
        await _set.AddRangeAsync(entities);
    }


    public void Remove(TEntity entity)
    {
        _set.Remove(entity);
    }


    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _set.RemoveRange(entities);
    }


    public void RemoveRange(params TEntity[] entities)
    {
        _set.RemoveRange(entities);
    }

    public async Task<bool> AnyAsync() => await _set.AnyAsync();

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter)
        => await _set.AnyAsync(filter);

    public async Task<int> CountAsync() => await _set.CountAsync();

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter)
        => await _set.CountAsync(filter);

    public async Task<int> PageCountAsync(int pageSize = 20)
    {
        var count = await CountAsync();
        var pageCount = count / pageSize;

        if (pageCount == 0 || pageCount % pageSize > 0)
            ++pageCount;

        return pageCount;
    }

    public async Task<bool> ValidPage(int page, int pageSize = 20)
    {
        if (page < 1)
            return false;

        var pageCount = await PageCountAsync(pageSize);

        return pageCount >= page;
    }
}
