using Ardalis.Result;
using Domain.Entities;

namespace Application.Interfaces;

public interface IFolderServices
{
    Result<List<FolderItem>> GetSubFolders(string path);
    Result<List<FileItem>> GetFiles(string path);
    Result<string> ReadFile(string filePath);

    Result<string> CreateFolder(string path);
    Result<string> RenameFolder(string oldPath, string newPath);
    Result<string> DeleteFolder(string path);

    // New async methods for FolderController
    Task<FolderItemResponse?> GetFolderByIdAsync(Guid id);
    Task<FolderContentsResult> GetFolderContentsAsync(Guid id);
    Task<FolderItemResponse> CreateFolderAsync(CreateFolderRequest request);
    Task UpdateFolderAsync(Guid id, UpdateFolderRequest request);
    Task DeleteFolderAsync(Guid id);
    Task MoveFolderAsync(Guid id, Guid destinationFolderId);
}

public record FolderItemResponse(Guid Id, string Name, Guid? ParentFolderId, DateTime CreatedAt);

public record FolderContentsResult(
    ICollection<FolderItemResponse> Folders,
    ICollection<FileItem> Files
);

public record CreateFolderRequest(string Name, Guid? ParentFolderId);

public record UpdateFolderRequest(string Name);
