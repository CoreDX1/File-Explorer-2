using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(FileExplorerDbContext context)
        : base(context) { }

    public async Task<RefreshToken?> GetActiveAsync(int userId)
    {
        return await Queryable()
            .Where(rt => rt.UserId == userId && rt.IsActive)
            .Include(rt => rt.User)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RefreshToken>> GetUserTokensAsync(int userId)
    {
        return await Queryable()
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.Created)
            .ToListAsync();
    }

    // IMPLEMENTACIÓN CORREGIDA
    public void UpdateAsync(RefreshToken refreshToken)
    {
        refreshToken.TrackingState = TrackingState.Modified;
        ApplyChanges(refreshToken);
    }
}
