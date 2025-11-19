using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FolderController : ControllerBase
{
    private readonly IFolderServices _folderServices;
    private readonly ILogger<FolderController> _logger;

    public FolderController(IFolderServices folderServices, ILogger<FolderController> logger)
    {
        _folderServices = folderServices;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetFolders([FromQuery] string path = "")
    {
        _logger.LogInformation("Getting folders for path: {Path}", path);
        
        var result = _folderServices.GetSubFolders(path);
        
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get folders for path: {Path}", path);
            return NotFound(new { message = result.Errors });
        }
        
        _logger.LogInformation("Retrieved {Count} folders for path: {Path}", result.Value.Count, path);
        return Ok(result.Value);
    }

    [HttpGet("files")]
    public IActionResult GetFiles([FromQuery] string path = "")
    {
        _logger.LogInformation("Getting files for path: {Path}", path);
        
        var result = _folderServices.GetFiles(path);
        
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get files for path: {Path}", path);
            return NotFound(new { message = result.Errors });
        }
        
        _logger.LogInformation("Retrieved {Count} files for path: {Path}", result.Value.Count, path);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFolder(Guid id)
    {
        var folder = await _folderServices.GetFolderByIdAsync(id);
        return folder != null ? Ok(folder) : NotFound();
    }

    [HttpGet("{id:guid}/contents")]
    public async Task<IActionResult> GetFolderContents(Guid id)
    {
        var contents = await _folderServices.GetFolderContentsAsync(id);
        return Ok(contents);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
    {
        var folder = await _folderServices.CreateFolderAsync(request);
        return CreatedAtAction(nameof(GetFolder), new { id = folder.Id }, folder);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFolder(Guid id, [FromBody] UpdateFolderRequest request)
    {
        await _folderServices.UpdateFolderAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFolder(Guid id)
    {
        await _folderServices.DeleteFolderAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/move")]
    public async Task<IActionResult> MoveFolder(Guid id, [FromBody] MoveFolderRequest request)
    {
        await _folderServices.MoveFolderAsync(id, request.DestinationFolderId);
        return NoContent();
    }
}

public record MoveFolderRequest(Guid DestinationFolderId);
