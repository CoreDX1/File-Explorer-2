using System.Data;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWorkAsync, IDisposable
{
    private readonly DbContext _context;
    private IDbContextTransaction? _transaction;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private bool _disposed;

    public UnitOfWork(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IRefreshTokenRepository RefreshTokenRepository =>
        _refreshTokenRepository ??= new RefreshTokenRepository((FileExplorerDbContext)_context);

    public int SaveChanges() => _context.SaveChanges();

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);

    public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified) =>
        _transaction = _context.Database.BeginTransaction(isolationLevel);

    public bool Commit()
    {
        _transaction?.Commit();
        return true;
    }

    public void Rollback() => _transaction?.Rollback();

    public void Dispose()
    {
        if (_disposed)
            return;
        _transaction?.Dispose();
        _context?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
