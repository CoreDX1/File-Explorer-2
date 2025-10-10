using TrackableEntities.Common.Core;

namespace Application.Interface;

public interface IService<TEntity>
    where TEntity : ITrackable
{
    TEntity Find(params object[] keyValues);
    IQueryable<TEntity> SelectQuery(string query, params object[] parameters);
    void Insert(TEntity entity);
    void InsertRange(IEnumerable<TEntity> entities);
    void ApplyChanges(TEntity entity);
    void Update(TEntity entity);
    void Delete(object id);
    void Delete(TEntity entity);
    Task<TEntity> FindAsync(params object[] keyValues);
    Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
    Task<bool> DeleteAsync(params object[] keyValues);
    Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    IQueryable<TEntity> Queryable();
}
