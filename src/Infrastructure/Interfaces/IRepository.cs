using TrackableEntities.Common.Core;

namespace Infrastructure.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class, ITrackable
{
    TEntity Find(params object[] keyValues);
    IQueryable<TEntity> SelectQuery(string query, params object[] parameters);
    void Insert(TEntity entity, bool traverseGraph = true);
    void ApplyChanges(TEntity entity);
    void InsertRange(IEnumerable<TEntity> entities, bool traverseGraph = true);

    void Update(TEntity entity, bool traverseGraph = true);
    void Delete(params object[] keyValues);
    void Delete(TEntity entity);
    IQueryable<TEntity> Queryable();
    // IRepository<T> GetRepository<T>()
    // where T : class, ITrackable;
}
