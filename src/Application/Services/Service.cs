using Application.Interface;
using Infrastructure.Interface;
using Infrastructure.Repositories;
using TrackableEntities.Common.Core;

namespace Application.Services;

public class Service<TEntity> : IService<TEntity>
    where TEntity : Entity, ITrackable
{
    private readonly IRepositoryAsync<TEntity> _repository;

    protected Service(IRepositoryAsync<TEntity> repository)
    {
        _repository = repository;
    }

    public virtual TEntity Find(params object[] keyValues)
    {
        return _repository.Find(keyValues);
    }

    public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
    {
        return _repository.SelectQuery(query, parameters).AsQueryable();
    }

    public virtual void Insert(TEntity entity)
    {
        _repository.Insert(entity);
    }

    public virtual void ApplyChanges(TEntity entity)
    {
        _repository.ApplyChanges(entity);
    }

    public virtual void InsertRange(IEnumerable<TEntity> entities)
    {
        _repository.InsertRange(entities);
    }

    public virtual void Update(TEntity entity)
    {
        _repository.Update(entity);
    }

    public virtual void Delete(object id)
    {
        _repository.Delete(id);
    }

    public virtual void Delete(TEntity entity)
    {
        _repository.Delete(entity);
    }

    public virtual async Task<TEntity> FindAsync(params object[] keyValues)
    {
        return await _repository.FindAsync(keyValues);
    }

    public virtual async Task<TEntity> FindAsync(
        CancellationToken cancellationToken,
        params object[] keyValues
    )
    {
        return await _repository.FindAsync(cancellationToken, keyValues);
    }

    public virtual async Task<bool> DeleteAsync(params object[] keyValues)
    {
        return await DeleteAsync(CancellationToken.None, keyValues);
    }

    public virtual async Task<bool> DeleteAsync(
        CancellationToken cancellationToken,
        params object[] keyValues
    )
    {
        return await _repository.DeleteAsync(cancellationToken, keyValues);
    }

    public IQueryable<TEntity> Queryable()
    {
        return _repository.Queryable();
    }
}
