using System.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Interfaces;

public interface IUnitOfWork
{
    IRefreshTokenRepository RefreshTokenRepository { get; }
    int SaveChanges();
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    bool Commit();
    void Rollback();
}
