using System.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TrackableEntities.Common.Core;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWorkAsync, IDisposable
{
    private readonly DbContext _context;
    private IDbContextTransaction? Transaction;
    private Dictionary<string, dynamic> Repositories;
    private bool _disposed;

    public UnitOfWork(DbContext context)
    {
        _context = context;
        Repositories = new Dictionary<string, dynamic>();
    }

    public int? CommandTimeout { get; set; }

    public virtual IRepository<TEntity> Repository<TEntity>()
        where TEntity : class, ITrackable
    {
        return RepositoryAsync<TEntity>();
    }

    public virtual int SaveChanges() => _context.SaveChanges();

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public virtual IRepositoryAsync<TEntity> RepositoryAsync<TEntity>()
        where TEntity : class, ITrackable
    {
        if (Repositories == null)
        {
            Repositories = new Dictionary<string, dynamic>();
        }

        var type = typeof(TEntity).Name;

        if (Repositories.ContainsKey(type))
        {
            return (IRepositoryAsync<TEntity>)Repositories[type];
        }

        var repositoryType = typeof(Repository<>);
        var instance = Activator.CreateInstance(
            repositoryType.MakeGenericType(typeof(TEntity)),
            _context
        );

        if (instance != null)
        {
            Repositories.Add(type, instance);
            return (IRepositoryAsync<TEntity>)instance;
        }

        throw new InvalidOperationException($"Could not create repository for {type}");
    }

    public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
    {
        return _context.Database.ExecuteSqlRaw(sql, parameters);
    }

    public virtual async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters).ConfigureAwait(false);
    }

    public virtual async Task<int> ExecuteSqlCommandAsync(
        string sql,
        CancellationToken cancellationToken,
        params object[] parameters
    )
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters).ConfigureAwait(false);
    }

    public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
    {
        Transaction = _context.Database.BeginTransaction(isolationLevel);
    }

    public virtual bool Commit()
    {
        Transaction?.Commit();
        return true;
    }

    public virtual void Rollback()
    {
        Transaction?.Rollback();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Transaction?.Dispose();
                _context?.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
