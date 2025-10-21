using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly ILogger<PermissionController> _logger;

    public PermissionController(ILogger<PermissionController> logger)
    {
        _logger = logger;
    }

    [HttpGet("files/{fileId:guid}")]
    public async Task<IActionResult> GetFilePermissions(Guid fileId)
    {
        // TODO: Implement get file permissions logic
        var permissions = Array.Empty<object>();
        return Ok(permissions);
    }

    [HttpPost("files/{fileId:guid}/grant")]
    public async Task<IActionResult> GrantFilePermission(
        Guid fileId,
        [FromBody] GrantPermissionRequest request
    )
    {
        // TODO: Implement grant permission logic
        return Ok(new { message = "Permission granted successfully" });
    }

    [HttpDelete("{permissionId:guid}")]
    public async Task<IActionResult> RevokePermission(Guid permissionId)
    {
        // TODO: Implement revoke permission logic
        return NoContent();
    }

    [HttpGet("folders/{folderId:guid}")]
    public async Task<IActionResult> GetFolderPermissions(Guid folderId)
    {
        // TODO: Implement get folder permissions logic
        var permissions = Array.Empty<object>();
        return Ok(permissions);
    }

    [HttpPost("folders/{folderId:guid}/grant")]
    public async Task<IActionResult> GrantFolderPermission(
        Guid folderId,
        [FromBody] GrantPermissionRequest request
    )
    {
        // TODO: Implement grant folder permission logic
        return Ok(new { message = "Permission granted successfully" });
    }

    [HttpGet]
}

public record GrantPermissionRequest(
    string UserEmail,
    string Permission, // "read", "write", "admin"
    DateTime? ExpiresAt
);
