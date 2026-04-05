using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class FileServices : IFileServices
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<FileServices> _logger;
    private readonly string _basePath;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileServices(
        IFileRepository fileRepository,
        ILogger<FileServices> logger,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _fileRepository = fileRepository;
        _logger = logger;
        var configPath = configuration["FileStorage:ContainerPath"] ?? "CONTENEDOR";
        // Resolve relative path to absolute path from working directory
        _basePath = Path.IsPathRooted(configPath)
            ? configPath
            : Path.Combine(Directory.GetCurrentDirectory(), "..", "..", configPath);
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid GetUserId()
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

    private string GetUserFolderPath()
    {
        var userId = GetUserId();
        var userFolderPath = Path.Combine(_basePath, userId.ToString());

        if (!Directory.Exists(userFolderPath))
        {
            Directory.CreateDirectory(userFolderPath);
        }

        return userFolderPath;
    }

    public async Task<bool> CreateFileAsync(CreateFileRequest createFile)
    {
        try
        {
            var fileEntity = new FileItem(
                createFile.FileName,
                string.Empty,
                0,
                DateTime.UtcNow,
                DateTime.UtcNow
            )
            {
                Id = createFile.Id,
                ParentFolderId = null,
            };

            await _fileRepository.CreateAsync(fileEntity);
            _logger.LogInformation(
                "File created: {FileName} with ID: {Id}",
                createFile.FileName,
                createFile.Id
            );
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating file: {FileName}", createFile.FileName);
            return false;
        }
    }

    public Task<ShareLinkResult> CreateShareLinkAsync(Guid id, ShareFileRequest request)
    {
        // TODO: Implement share link creation with database persistence
        throw new NotImplementedException("Share link creation not yet implemented");
    }

    public async Task DeleteFileAsync(Guid id)
    {
        _logger.LogInformation("Deleting file with ID: {Id}", id);
        await _fileRepository.DeleteAsync(id);
        _logger.LogInformation("File deleted successfully: {Id}", id);
    }

    public async Task<FileDownloadResult?> DownloadFileAsync(Guid id)
    {
        var fileItem = await _fileRepository.GetByIdAsync(id);
        if (fileItem == null)
        {
            _logger.LogWarning("File not found for download: {Id}", id);
            return null;
        }

        if (!File.Exists(fileItem.Path))
        {
            _logger.LogWarning("Physical file not found: {Id}, Path: {Path}", id, fileItem.Path);
            return null;
        }

        var stream = new FileStream(fileItem.Path, FileMode.Open, FileAccess.Read);
        return new FileDownloadResult(stream, fileItem.ContentType, fileItem.Name);
    }

    public async Task<FileItem?> GetFileByIdAsync(Guid id)
    {
        return await _fileRepository.GetByIdAsync(id);
    }

    public ICollection<FileItem> GetFiles(string path)
    {
        var userFolderPath = GetUserFolderPath();
        return _fileRepository.GetFilesAsync(userFolderPath);
    }

    public string ReadFile(string filePath)
    {
        return _fileRepository.ReadFileAsync(filePath);
    }

    public async Task<FileUploadResult> UploadFileAsync(IFormFile file, Guid? folderId)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required", nameof(file));
        }

        if (file.Length > 100 * 1024 * 1024) // 100 MB limit
        {
            throw new ArgumentException("File size exceeds the 100 MB limit", nameof(file));
        }

        var fileId = Guid.NewGuid();
        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);
        var contentType = file.ContentType;

        var storagePath = $"uploads/{DateTime.UtcNow:yyyy/MM/dd}/{fileId}{extension}";
        var physicalPath = Path.Combine("wwwroot", storagePath);

        Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);

        using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream).ConfigureAwait(false);
        }

        var fileEntity = new FileItem(
            file.FileName,
            physicalPath,
            file.Length,
            DateTime.UtcNow,
            DateTime.UtcNow
        )
        {
            Id = fileId,
            StorageFileName = fileId.ToString() + extension,
            ContentType = contentType,
            ParentFolderId = folderId,
        };

        await _fileRepository.CreateAsync(fileEntity);

        _logger.LogInformation("File uploaded: {FileName} with ID: {Id}", file.FileName, fileId);

        return new FileUploadResult(fileId, file.FileName, file.Length);
    }
}
