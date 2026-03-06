using Domain.Entities;

namespace Domain.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    public Task<RefreshToken?> GetActiveAsync(int userId);
    public Task<List<RefreshToken>> GetUserTokensAsync(int userId);
    public void UpdateAsync(RefreshToken refreshToken);
}
