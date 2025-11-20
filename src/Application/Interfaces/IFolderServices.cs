using Ardalis.Result;
using Domain.Entities;

namespace Application.Interfaces;

public interface IFolderServices
{
    Result<List<DirectoryItem>> GetSubFolders(string path);
    Result<List<FileItem>> GetFiles(string path);
    Result<string> ReadFile(string filePath);

    Result<string> CreateFolder(string path);
    Result<string> RenameFolder(string oldPath, string newPath);
    Result<string> DeleteFolder(string path);

    // New async methods for FolderController
    Task<FolderItem?> GetFolderByIdAsync(Guid id);
    Task<FolderContentsResult> GetFolderContentsAsync(Guid id);
    Task<FolderItem> CreateFolderAsync(CreateFolderRequest request);
    Task UpdateFolderAsync(Guid id, UpdateFolderRequest request);
    Task DeleteFolderAsync(Guid id);
    Task MoveFolderAsync(Guid id, Guid destinationFolderId);
}

public record FolderItem(Guid Id, string Name, Guid? ParentFolderId, DateTime CreatedAt);

public record FolderContentsResult(ICollection<FolderItem> Folders, ICollection<FileItem> Files);

public record CreateFolderRequest(string Name, Guid? ParentFolderId);

public record UpdateFolderRequest(string Name);
