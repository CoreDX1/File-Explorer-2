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

public class FolderServices : IFolderServices
{
    private readonly IFolderRepository _folderRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _containerPath;

    public FolderServices(
        IFolderRepository folderRepository,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _folderRepository = folderRepository;
        _httpContextAccessor = httpContextAccessor;
        _containerPath =
            configuration["FileStorage:ContainerPath"]
            ?? @"C:\Users\christian\Desktop\Project\File-Explorer\CONTENEDOR";
    }

    private int GetAuthenticatedUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(
            ClaimTypes.NameIdentifier
        );
        return int.Parse(
            userIdClaim?.Value ?? throw new UnauthorizedAccessException("User not authenticated"),
            System.Globalization.CultureInfo.InvariantCulture
        );
    }

    private string ResolveUserStoragePath()
    {
        var userId = GetAuthenticatedUserId();
        var userStoragePath = Path.Combine(_containerPath, $"user_{userId}");
        Directory.CreateDirectory(userStoragePath);
        return userStoragePath;
    }

    public Result<List<DirectoryItem>> GetSubFolders(string path)
    {
        var userStoragePath = ResolveUserStoragePath();
        var absolutePath = Path.Combine(userStoragePath, path);
        var folders = _folderRepository.GetSubFolders(absolutePath);
        return Result.Success(folders.ToList());
    }

    public Result<List<FileItem>> GetFiles(string path)
    {
        var userStoragePath = ResolveUserStoragePath();
        var absolutePath = Path.Combine(userStoragePath, path);
        var files = _folderRepository.GetFiles(absolutePath);
        return Result.Success(files.ToList());
    }

    public Result<string> ReadFile(string filePath)
    {
        var userStoragePath = ResolveUserStoragePath();
        var absoluteFilePath = Path.Combine(userStoragePath, filePath);
        var fileContent = _folderRepository.ReadFile(absoluteFilePath);
        return Result.Success(fileContent, "File read successfully.");
    }

    public Result<string> CreateFolder(string path)
    {
        var userStoragePath = ResolveUserStoragePath();
        var absoluteFolderPath = Path.Combine(userStoragePath, path);
        var wasCreated = _folderRepository.CreateFolder(absoluteFolderPath);

        if (!wasCreated)
            return Result.NotFound("Directory could not be created.");

        return Result.Success("Directory created successfully.");
    }

    public Result<string> RenameFolder(string oldPath, string newPath)
    {
        var userStoragePath = ResolveUserStoragePath();
        var absoluteOldPath = Path.Combine(userStoragePath, oldPath);
        var absoluteNewPath = Path.Combine(userStoragePath, newPath);
        var wasRenamed = _folderRepository.RenameFolder(absoluteOldPath, absoluteNewPath);

        if (!wasRenamed)
            return Result.NotFound("Directory could not be renamed.");

        return Result.Success("Directory renamed successfully.");
    }

    public Result<string> DeleteFolder(string path)
    {
        var userStoragePath = ResolveUserStoragePath();
        var absoluteFolderPath = Path.Combine(userStoragePath, path);
        var wasDeleted = _folderRepository.DeleteFolder(absoluteFolderPath);

        if (!wasDeleted)
            return Result.NotFound("Directory could not be deleted.");

        return Result.Success("Directory deleted successfully.");
    }

    public Task<FolderItem?> GetFolderByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<FolderContentsResult> GetFolderContentsAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<FolderItem> CreateFolderAsync(CreateFolderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var userStoragePath = ResolveUserStoragePath();
        var folderPath = Path.Combine(userStoragePath, request.Name);

        var wasCreated = _folderRepository.CreateFolder(folderPath);

        if (!wasCreated)
            throw new InvalidOperationException("Directory could not be created");

        var folderItem = new FolderItem(
            Guid.NewGuid(),
            request.Name,
            request.ParentFolderId,
            DateTime.UtcNow
        );

        return Task.FromResult(folderItem);
    }

    public Task UpdateFolderAsync(Guid id, UpdateFolderRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFolderAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task MoveFolderAsync(Guid id, Guid destinationFolderId)
    {
        throw new NotImplementedException();
    }
}
