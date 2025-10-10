using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity>, IRepositoryAsync<TEntity>
    where TEntity : Entity
{
    protected readonly FileExplorerDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(FileExplorerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    // Find Methods
    public TEntity Find(params object[] keyValues)
    {
        return _dbSet.Find(keyValues);
    }

    public async Task<TEntity> FindAsync(params object[] keyValues)
    {
        return await _dbSet.FindAsync(keyValues);
    }

    public async Task<TEntity> FindAsync(
        CancellationToken cancellationToken,
        params object[] keyValues
    )
    {
        return await _dbSet.FindAsync(keyValues, cancellationToken);
    }

    // Insert Methods
    public void Insert(TEntity entity, bool traverseGraph = true)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Add(entity);
    }

    public void InsertRange(IEnumerable<TEntity> entities, bool traverseGraph = true)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        _dbSet.AddRange(entities);
    }

    // Update Methods
    public void Update(TEntity entity, bool traverseGraph = true)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Update(entity);
    }

    public void ApplyChanges(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _context.Entry(entity).State = EntityState.Modified;
    }

    // Delete Methods
    public void Delete(params object[] keyValues)
    {
        var entity = _dbSet.Find(keyValues);
        if (entity != null)
            _dbSet.Remove(entity);
    }

    public void Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
    }

    public async Task<bool> DeleteAsync(params object[] keyValues)
    {
        return await DeleteAsync(CancellationToken.None, keyValues);
    }

    public async Task<bool> DeleteAsync(
        CancellationToken cancellationToken,
        params object[] keyValues
    )
    {
        var entity = await _dbSet.FindAsync(keyValues, cancellationToken);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        return true;
    }

    // Query Methods
    public IQueryable<TEntity> Queryable()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
    {
        return _dbSet.FromSqlRaw(query, parameters);
    }

    public async Task<IEnumerable<TEntity>> SelectQueryAsync(
        string query,
        params object[] parameters
    )
    {
        return await SelectQueryAsync(query, CancellationToken.None, parameters);
    }

    public async Task<IEnumerable<TEntity>> SelectQueryAsync(
        string query,
        CancellationToken cancellationToken,
        params object[] parameters
    )
    {
        return await _dbSet.FromSqlRaw(query, parameters).ToListAsync(cancellationToken);
    }
}
