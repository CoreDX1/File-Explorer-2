namespace Domain.Interfaces;

public interface IUnitOfWorkAsync : IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
