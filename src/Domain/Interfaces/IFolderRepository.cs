using Domain.Entities;

namespace Domain.Interfaces;

public interface IFolderRepository
{
    // File system operations
    ICollection<DirectoryItem> GetSubFolders(string path);
    ICollection<FileItem> GetFiles(string path);
    string ReadFile(string filePath);
    bool CreateFolder(string path);
    bool RenameFolder(string oldPath, string newPath);
    bool DeleteFolder(string path);

    // Database operations
    IQueryable<FolderItem> Queryable();
    Task<FolderItem?> GetByIdAsync(Guid id);
    Task<ICollection<FolderItem>> GetByParentIdAsync(Guid? parentId);
    Task<FolderItem> CreateAsync(FolderItem folderItem);
    Task UpdateAsync(FolderItem folderItem);
    Task DeleteAsync(Guid id);
    Task MoveAsync(Guid folderId, Guid newParentId);
    Task<ICollection<FolderItem>> GetSubFoldersByParentIdAsync(Guid? parentId);
    Task<ICollection<FileItem>> GetFilesByFolderIdAsync(Guid folderId);
}
