using System.Data;

namespace Domain.Interfaces;

public interface IUnitOfWork
{
    IRefreshTokenRepository RefreshTokenRepository { get; }
    IUserRepository UserRepository { get; }

    int SaveChanges();
    void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
    bool Commit();
    void Rollback();
}
