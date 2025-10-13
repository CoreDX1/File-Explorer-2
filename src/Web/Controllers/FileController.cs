using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileServices _fileServices;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileServices fileServices, ILogger<FileController> logger)
    {
        _fileServices = fileServices;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var file = await _fileServices.GetFileByIdAsync(id);
        return file != null ? Ok(file) : NotFound();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] Guid? folderId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var result = await _fileServices.UploadFileAsync(file, folderId);
        return Ok(result);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var fileStream = await _fileServices.DownloadFileAsync(id);
        if (fileStream == null)
            return NotFound();

        return File(fileStream.Stream, fileStream.ContentType, fileStream.FileName);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFile(Guid id)
    {
        await _fileServices.DeleteFileAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/share")]
    public async Task<IActionResult> ShareFile(Guid id, [FromBody] ShareFileRequest request)
    {
        var shareLink = await _fileServices.CreateShareLinkAsync(id, request);
        return Ok(shareLink);
    }
}

public record ShareFileRequest(string Email, string Permission, DateTime? ExpiresAt);
