namespace Application.Services;

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces;
using Ardalis.Result;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class FolderServices : IFolderServices
{
    private readonly IFolderRepository _folderRepository;
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<FolderServices> _logger;
    private readonly string _containerPath;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FolderServices(
        IFolderRepository folderRepository,
        IFileRepository fileRepository,
        IConfiguration configuration,
        ILogger<FolderServices> logger,
        IHttpContextAccessor httpContextAccessor
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _folderRepository = folderRepository;
        _fileRepository = fileRepository;
        _logger = logger;
        var configPath = configuration["FileStorage:ContainerPath"] ?? "CONTENEDOR";
        // Resolve relative path to absolute path from working directory
        _containerPath = Path.IsPathRooted(configPath)
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
        var userFolderPath = Path.Combine(_containerPath, userId.ToString());

        // Create user folder if it doesn't exist
        if (!Directory.Exists(userFolderPath))
        {
            Directory.CreateDirectory(userFolderPath);
        }

        return userFolderPath;
    }

    public Result<List<DirectoryItem>> GetSubFolders(string path)
    {
        try
        {
            var userFolderPath = GetUserFolderPath();
            var folders = _folderRepository.GetSubFolders(userFolderPath);
            return Result.Success(folders.ToList());
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Failed to get subfolders for path: {Path}", path);
            return Result<List<DirectoryItem>>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subfolders for path: {Path}", path);
            return Result<List<DirectoryItem>>.Error(
                new ErrorList(new[] { "An error occurred while retrieving folders" }, null)
            );
        }
    }

    public Result<List<FileItem>> GetFiles(string path)
    {
        try
        {
            var userFolderPath = GetUserFolderPath();
            var files = _folderRepository.GetFiles(userFolderPath);
            return Result.Success(files.ToList());
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Failed to get files for path: {Path}", path);
            return Result<List<FileItem>>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting files for path: {Path}", path);
            return Result<List<FileItem>>.Error(
                new ErrorList(new[] { "An error occurred while retrieving files" }, null)
            );
        }
    }

    public Result<string> ReadFile(string filePath)
    {
        try
        {
            var fileContent = _folderRepository.ReadFile(filePath);
            return Result.Success(fileContent);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Failed to read file: {FilePath}", filePath);
            return Result<string>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            return Result<string>.Error(
                new ErrorList(new[] { "An error occurred while reading file" }, null)
            );
        }
    }

    public Result<string> CreateFolder(string path)
    {
        try
        {
            var wasCreated = _folderRepository.CreateFolder(path);

            if (!wasCreated)
                return Result<string>.NotFound("Directory could not be created.");

            return Result.Success("Directory created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating folder: {Path}", path);
            return Result<string>.Error(
                new ErrorList(new[] { "An error occurred while creating folder" }, null)
            );
        }
    }

    public Result<string> RenameFolder(string oldPath, string newPath)
    {
        try
        {
            var wasRenamed = _folderRepository.RenameFolder(oldPath, newPath);

            if (!wasRenamed)
                return Result<string>.NotFound("Directory could not be renamed.");

            return Result.Success("Directory renamed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error renaming folder from {OldPath} to {NewPath}",
                oldPath,
                newPath
            );
            return Result<string>.Error(
                new ErrorList(new[] { "An error occurred while renaming folder" }, null)
            );
        }
    }

    public Result<string> DeleteFolder(string path)
    {
        try
        {
            var wasDeleted = _folderRepository.DeleteFolder(path);

            if (!wasDeleted)
                return Result<string>.NotFound("Directory could not be deleted.");

            return Result.Success("Directory deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting folder: {Path}", path);
            return Result<string>.Error(
                new ErrorList(new[] { "An error occurred while deleting folder" }, null)
            );
        }
    }

    public async Task<FolderItemResponse?> GetFolderByIdAsync(Guid id)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
        {
            _logger.LogWarning("Folder not found with ID: {Id}", id);
            return null;
        }

        return new FolderItemResponse(
            folder.Id,
            folder.Name,
            folder.ParentFolderId,
            folder.CreatedAt
        );
    }

    public async Task<FolderContentsResult> GetFolderContentsAsync(Guid id)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
        {
            _logger.LogWarning("Folder not found with ID: {Id}", id);
            return new FolderContentsResult(
                Array.Empty<FolderItemResponse>(),
                Array.Empty<FileItem>()
            );
        }

        var subFolders = await _folderRepository.GetSubFoldersByParentIdAsync(id);
        var files = await _fileRepository.GetByFolderIdAsync(id);

        var folderResponses = subFolders
            .Select(f => new FolderItemResponse(f.Id, f.Name, f.ParentFolderId, f.CreatedAt))
            .ToList();

        return new FolderContentsResult(folderResponses, files);
    }

    public async Task<FolderItemResponse> CreateFolderAsync(CreateFolderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = GetUserId();
        var userFolderPath = GetUserFolderPath();
        var folderId = Guid.NewGuid();

        // Build physical path: CONTENEDOR/{userId}/{folderName}
        var physicalFolderPath = Path.Combine(userFolderPath, request.Name);

        var folderItem = new FolderItem(
            request.Name,
            physicalFolderPath,
            0,
            DateTime.UtcNow,
            DateTime.UtcNow
        )
        {
            Id = folderId,
            ParentFolderId = request.ParentFolderId,
        };

        await _folderRepository.CreateAsync(folderItem);

        _logger.LogInformation(
            "Folder created for user {UserId}: {Name} with ID: {Id}",
            userId,
            request.Name,
            folderId
        );

        return new FolderItemResponse(
            folderItem.Id,
            folderItem.Name,
            folderItem.ParentFolderId,
            folderItem.CreatedAt
        );
    }

    public async Task UpdateFolderAsync(Guid id, UpdateFolderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
        {
            _logger.LogWarning("Folder not found for update with ID: {Id}", id);
            throw new KeyNotFoundException($"Folder with ID {id} not found");
        }

        var oldPath = folder.Path;
        var newPath = Path.Combine(Path.GetDirectoryName(oldPath) ?? "", request.Name);

        // Rename physical folder if exists
        if (Directory.Exists(oldPath))
        {
            _folderRepository.RenameFolder(oldPath, newPath);
        }

        folder.Name = request.Name;
        folder.Path = newPath;

        await _folderRepository.UpdateAsync(folder);

        _logger.LogInformation("Folder updated: ID {Id}, New Name: {Name}", id, request.Name);
    }

    public async Task DeleteFolderAsync(Guid id)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
        {
            _logger.LogWarning("Folder not found for delete with ID: {Id}", id);
            return;
        }

        await _folderRepository.DeleteAsync(id);

        _logger.LogInformation("Folder deleted: ID {Id}", id);
    }

    public async Task MoveFolderAsync(Guid id, Guid destinationFolderId)
    {
        await _folderRepository.MoveAsync(id, destinationFolderId);
        _logger.LogInformation(
            "Folder moved: ID {Id} to destination {DestinationId}",
            id,
            destinationFolderId
        );
    }
}
