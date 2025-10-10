using TrackableEntities.Common.Core;

namespace Infrastructure.Interface;

public interface IUnitOfWorkAsync : IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    IRepositoryAsync<TEntity> RepositoryAsync<TEntity>()
        where TEntity : class, ITrackable;
}
