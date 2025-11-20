using System.Data;
using TrackableEntities.Common.Core;

namespace Infrastructure.Interfaces;

public interface IUnitOfWork
{
    int SaveChanges();
    int ExecuteSqlCommand(string sql, params object[] parameters);
    IRepository<TEntity> Repository<TEntity>()
        where TEntity : class, ITrackable;
    int? CommandTimeout { get; set; }
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    bool Commit();
    void Rollback();
}
