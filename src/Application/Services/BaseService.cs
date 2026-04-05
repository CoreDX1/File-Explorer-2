using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public abstract class BaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _basePath;

    protected BaseService(IHttpContextAccessor httpContextAccessor, string basePath)
    {
        _httpContextAccessor = httpContextAccessor;
        _basePath = basePath;
    }

    protected Guid GetUserId()
    {
        var userIdClaim = _httpContextAccessor
            .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return Guid.Parse(userIdClaim);
    }

    protected string GetUserFolderPath()
    {
        var userId = GetUserId();
        var userFolderPath = Path.Combine(_basePath, userId.ToString());

        if (!Directory.Exists(userFolderPath))
        {
            Directory.CreateDirectory(userFolderPath);
        }

        return userFolderPath;
    }
}
