using Domain.Entities;

namespace Domain.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    public Task<RefreshToken?> GetActiveAsync(Guid userId);
    public Task<List<RefreshToken>> GetUserTokensAsync(Guid userId);
    public void UpdateAsync(RefreshToken refreshToken);
}
