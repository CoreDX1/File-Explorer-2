using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly FileExplorerDbContext _context;
    private readonly string _basePath;

    public FileRepository(FileExplorerDbContext context, IConfiguration configuration)
    {
        _context = context;
        var configPath = configuration["FileStorage:ContainerPath"] ?? "CONTENEDOR";
        _basePath = Path.IsPathRooted(configPath)
            ? configPath
            : Path.Combine(Directory.GetCurrentDirectory(), "..", "..", configPath);
    }

    // File system operations
    public ICollection<FileItem> GetFilesAsync(string path)
    {
        // If path is already absolute, use it directly; otherwise combine with basePath
        string fullPath = Path.IsPathRooted(path) ? path : Path.Combine(_basePath, path);

        if (!Directory.Exists(fullPath))
            throw new ArgumentException($"The directory does not exist: {fullPath}");

        var files = new List<FileItem>();

        foreach (var file in Directory.GetFiles(fullPath))
        {
            var fileInfo = new FileInfo(file);
            files.Add(
                new FileItem(
                    name: Path.GetFileName(file),
                    path: file,
                    size: fileInfo.Length,
                    createdAt: fileInfo.CreationTime,
                    modifiedAt: fileInfo.LastWriteTime
                )
            );
        }

        return files;
    }

    public string ReadFileAsync(string filePath)
    {
        string fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
            throw new ArgumentException("File does not exist.");

        return File.ReadAllText(fullPath);
    }

    // Database operations
    public IQueryable<FileItem> Queryable()
    {
        return _context.FileItems.AsQueryable();
    }

    public async Task<FileItem?> GetByIdAsync(Guid id)
    {
        return await _context
            .FileItems.Include(f => f.ParentFolder)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<ICollection<FileItem>> GetByFolderIdAsync(Guid folderId)
    {
        return await _context.FileItems.Where(f => f.ParentFolderId == folderId).ToListAsync();
    }

    public async Task<FileItem> CreateAsync(FileItem fileItem)
    {
        _context.FileItems.Add(fileItem);
        await _context.SaveChangesAsync();
        return fileItem;
    }

    public async Task UpdateAsync(FileItem fileItem)
    {
        fileItem.ModifiedAt = DateTime.UtcNow;
        _context.FileItems.Update(fileItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var fileItem = await _context.FileItems.FindAsync(id);
        if (fileItem != null)
        {
            // Delete physical file if it exists
            if (!string.IsNullOrEmpty(fileItem.Path) && File.Exists(fileItem.Path))
            {
                File.Delete(fileItem.Path);
            }

            _context.FileItems.Remove(fileItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ICollection<FileItem>> SearchAsync(string searchTerm, Guid? folderId = null)
    {
        var query = _context.FileItems.AsQueryable();

        if (folderId.HasValue)
        {
            query = query.Where(f => f.ParentFolderId == folderId.Value);
        }

        return await query.Where(f => f.Name.Contains(searchTerm)).ToListAsync();
    }

    public async Task<long> GetTotalSizeByUserIdAsync(Guid userId)
    {
        return await _context
            .FileItems.Where(f => f.ParentFolder != null && f.ParentFolder.CreatedAt != null)
            .SumAsync(f => f.Size);
    }
}
