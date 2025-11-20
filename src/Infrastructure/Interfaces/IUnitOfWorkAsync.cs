using TrackableEntities.Common.Core;

namespace Infrastructure.Interfaces;

public interface IUnitOfWorkAsync : IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    IRepositoryAsync<TEntity> RepositoryAsync<TEntity>()
        where TEntity : class, ITrackable;
}
