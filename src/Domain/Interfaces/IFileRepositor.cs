using Domain.Entities;

namespace Domain.Interfaces;

public interface IFileRepository
{
    // File system operations
    ICollection<FileItem> GetFilesAsync(string path);
    string ReadFileAsync(string filePath);

    // Database operations
    IQueryable<FileItem> Queryable();
    Task<FileItem?> GetByIdAsync(Guid id);
    Task<ICollection<FileItem>> GetByFolderIdAsync(Guid folderId);
    Task<FileItem> CreateAsync(FileItem fileItem);
    Task UpdateAsync(FileItem fileItem);
    Task DeleteAsync(Guid id);
    Task<ICollection<FileItem>> SearchAsync(string searchTerm, Guid? folderId = null);
    Task<long> GetTotalSizeByUserIdAsync(Guid userId);
}
